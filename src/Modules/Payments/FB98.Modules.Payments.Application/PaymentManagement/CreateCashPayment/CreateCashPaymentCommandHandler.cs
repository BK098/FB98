using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Email;
using MassTransit;
using Refit;
using System.Text.Json;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	internal sealed class CreateCashPaymentCommandHandler : ICommandHandler<CreateCashPaymentCommand, ApiResult<object>>
	{
		private readonly IBookingApi _bookingApi;
		private readonly IBus _bus;
		private readonly ICouponRepository _couponRepository;
		private readonly IEmailSender _emailSender;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCashPaymentCommandHandler> _logger;
		private readonly IOrderApi _orderApi;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IUserApi _userApi;

		public CreateCashPaymentCommandHandler(
			IPaymentRepository paymentRepository,
			ILogger<CreateCashPaymentCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IBus bus,
			IUserApi userApi,
			IOrderApi orderApi,
			IBookingApi bookingApi,
			ICouponRepository couponRepository,
			IEmailSender emailSender)
		{
			_paymentRepository = paymentRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_bus = bus;
			_userApi = userApi;
			_orderApi = orderApi;
			_bookingApi = bookingApi;
			_couponRepository = couponRepository;
			_emailSender = emailSender;
		}

		public async Task<ApiResult<object>> Handle(CreateCashPaymentCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var filter = request.Filter;
			decimal amount = 0;
			var now = DateTime.UtcNow;
			try
			{
				if (model.OrderId == null && model.BookingId == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("OrderOrBookingRequired"));
				}

				BookingDetailDto? booking = null;
				OrderDetailDto? order = null;

				try
				{
					if (model.OrderId != null)
					{
						var orderResponse = await _orderApi.GetOrderDetailById(model.OrderId!.Value);
						if (orderResponse.IsSuccess)
						{
							order = orderResponse.Data;
						}

						amount += orderResponse.Data!.Amount;
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("Order: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				try
				{
					if (model.BookingId != null)
					{
						var bookingResponse = await _bookingApi.GetDetailBooking(model.BookingId!.Value);
						if (bookingResponse.IsSuccess)
						{
							booking = bookingResponse.Data;
						}

						amount += bookingResponse.Data!.Amount;
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("Booking: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				decimal discount = 0;
				if (!string.IsNullOrWhiteSpace(model.CouponCode))
				{
					var coupon = await _couponRepository.GetValidCouponAsync(model.CouponCode.Normalize().ToUpper().Trim(), amount, now);
					if (coupon == null)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("CouponInvalid"));
					}

					discount = coupon.CalculateDiscount(amount);
				}

				var email = string.Empty;
				var phoneNumber = string.Empty;
				var userId = Guid.Empty;
				try
				{
					if (filter != null)
					{
						var userResponse = await _userApi.GetUserProfile(filter);

						email = userResponse.Data!.Email;
						phoneNumber = userResponse.Data!.PhoneNumber;
						userId = Guid.Parse(userResponse.Data!.UserId);
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("User: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber) || userId == Guid.Empty)
				{
					return ApiResponseBuilder.Error<object>("User: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var finalAmount = amount - discount;
				var transaction = new PaymentTransaction
				{
					Email = email,
					PhoneNumber = phoneNumber,
					UserId = userId,
					OrderId = model.OrderId,
					BookingId = model.BookingId,
					Amount = finalAmount,
					SubAmount = amount,
					PaymentMethodId = PaymentMethodConstants.Cash
				};

				transaction.MarkSuccess();
				await _paymentRepository.CreateAsync(transaction);

				if (!string.IsNullOrWhiteSpace(transaction.CouponCode))
				{
					await _couponRepository.ApplyCouponAfterPaymentAsync(transaction.CouponCode, transaction.Id, transaction.Amount);
				}

				await SendMailAsync(transaction.Email, transaction.PhoneNumber, booking, order);
				await _bus.Publish(new VnPayPaymentCreatedEvent(userId, model.BookingId, model.OrderId), cancellationToken);
				return ApiResponseBuilder.Success<object>(transaction.Id, _localizedMessageService.GetLocalizedMessage("PaymentSuccessful"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create cash payment");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}

		private async Task SendMailAsync(string email, string phoneNumber, BookingDetailDto? booking, OrderDetailDto? order)
		{
			var emailBody = booking != null && order != null
				? GetBookingAndOrderTemplate(email, phoneNumber, booking, order)
				: booking != null
					? GetBookingOnlyTemplate(email, phoneNumber, booking)
					: GetOrderOnlyTemplate(email, phoneNumber, order!);

			var attachments = new List<(byte[], string)>();

			await _emailSender.SendEmailWithMultipleInlineAttachmentsAsync(
				email,
				"Xác nhận đơn hàng của bạn",
				emailBody,
				attachments);
		}

		private static string GetBookingOnlyTemplate(string email, string phone, BookingDetailDto booking)
		{
			var bookingData = new
			{
				bookingId = booking.Id,
				seatIds = booking.Seats.Select(bs => bs.SeatId).ToList()
			};
			var bookingJson = JsonSerializer.Serialize(bookingData);
			var bookingUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(bookingJson)}&size=200x200";
			var emailBody = $$"""
			                  <!DOCTYPE html>
			                  <html lang='vi'>
			                  <head>
			                      <meta charset='UTF-8'>
			                      <title>Xác nhận đặt vé xem phim</title>
			                      <style>
			                          body {
			                              font-family: Arial, sans-serif;
			                              background-color: #f4f4f4;
			                              color: #333;
			                          }
			                          .container {
			                              width: 600px;
			                              margin: auto;
			                              background-color: white;
			                              border-radius: 10px;
			                              overflow: hidden;
			                              box-shadow: 0 4px 8px rgba(0,0,0,0.2);
			                          }
			                          .header {
			                              background-color: #007bff;
			                              color: white;
			                              padding: 20px;
			                              text-align: center;
			                          }
			                          .content {
			                              padding: 20px;
			                              line-height: 1.6;
			                              margin-bottom: 20px;
			                          }
			                          .qr-code {
			                              text-align: center;
			                              margin-top: 20px;
			                          }
			                          table {
			                              width: 100%;
			                              border-collapse: collapse;
			                              margin-top: 15px;
			                          }
			                          table, th, td {
			                              border: 1px solid #ddd;
			                          }
			                          th, td {
			                              padding: 8px;
			                              text-align: left;
			                          }
			                          th {
			                              background-color: #f8f8f8;
			                          }
			                          .footer {
			                              background-color: #007bff;
			                              color: white;
			                              text-align: center;
			                              padding: 10px;
			                          }
			                      </style>
			                  </head>
			                  <body>
			                      <div class='container'>
			                          <div class='header'>
			                              <h2>Thông tin vé xem phim 🎬</h2>
			                          </div>
			                          <div class='content'>
			                              <p><strong>Email:</strong> {{email}}</p>
			                              <p><strong>Số điện thoại:</strong> {{phone}}</p>
			                              <p><strong>Phim:</strong> {{booking.MovieTitle}}</p>
			                              <p><strong>Rạp:</strong> {{booking.HallName}}</p>
			                              <p><strong>Thời gian chiếu:</strong> {{booking.ShowStart}}</p>
			                              <p><strong>Tổng tiền thanh toán:</strong> {{booking.Amount:N0}} VNĐ</p> 
			                  
			                              <table>
			                                  <thead>
			                                      <tr>
			                                          <th>Vị trí ghế</th>
			                                          <th>Loại ghế</th>
			                                      </tr>
			                                  </thead>
			                                  <tbody>
			                                      {{string.Join("", booking.Seats.Select(s => $"<tr><td>{s.SeatPosition}</td><td>{s.SeatTypeName}</td></tr>"))}}
			                                  </tbody>
			                              </table>
			                              <p>Hãy đưa mã QR này cho nhân viên khi bạn check-in tại rạp:</p>
			                              <div class='qr-code'>
			                                 <div style='display: flex; justify-content: center; align-items: center;'>
			                                     <div style='text-align: center;'>
			                                         <p>Vé xem phim:</p>
			                                         <img src='{{bookingUrl}}' alt='QR Code' width='200' height='200' />
			                                     </div>
			                                 </div>
			                             </div>
			                          </div>
			                          <div class='footer'>
			                              <p>Địa chỉ: Tòa nhà xxxx, Công Viên Phần Mềm Quang Trung, Tầng 3</p>
			                          </div>
			                      </div>
			                  </body>
			                  </html>
			                  """;
			return emailBody;
		}

		private static string GetOrderOnlyTemplate(string email, string phone, OrderDetailDto order)
		{
			var orderData = new
			{
				orderId = order.Id
			};
			var orderJson = JsonSerializer.Serialize(orderData);
			var orderUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(orderJson)}&size=200x200";

			var emailBody = $$"""
			                  <!DOCTYPE html>
			                  <html lang='vi'>
			                  <head>
			                      <meta charset='UTF-8'>
			                      <title>Xác nhận đặt vé xem phim</title>
			                      <style>
			                          body {
			                              font-family: Arial, sans-serif;
			                              background-color: #f4f4f4;
			                              color: #333;
			                          }
			                          .container {
			                              width: 600px;
			                              margin: auto;
			                              background-color: white;
			                              border-radius: 10px;
			                              overflow: hidden;
			                              box-shadow: 0 4px 8px rgba(0,0,0,0.2);
			                          }
			                          .header {
			                              background-color: #007bff;
			                              color: white;
			                              padding: 20px;
			                              text-align: center;
			                          }
			                          .content {
			                              padding: 20px;
			                              line-height: 1.6;
			                              margin-bottom: 20px;
			                          }
			                          .qr-code {
			                              text-align: center;
			                              margin-top: 20px;
			                          }
			                          table {
			                              width: 100%;
			                              border-collapse: collapse;
			                              margin-top: 15px;
			                          }
			                          table, th, td {
			                              border: 1px solid #ddd;
			                          }
			                          th, td {
			                              padding: 8px;
			                              text-align: left;
			                          }
			                          th {
			                              background-color: #f8f8f8;
			                          }
			                          .footer {
			                              background-color: #007bff;
			                              color: white;
			                              text-align: center;
			                              padding: 10px;
			                          }
			                      </style>
			                  </head>
			                  <body>
			                      <div class='container'>
			                          <div class='header'>
			                              <h2>Thông tin vé xem phim 🎬</h2>
			                          </div>
			                          <div class='content'>
			                              <p><strong>Email:</strong> {{email}}</p>
			                              <p><strong>Số điện thoại:</strong> {{phone}}</p>
			                              <p><strong>Tổng tiền thanh toán:</strong> {{order.Amount:N0}} VNĐ</p>
			                  
			                              <table>
			                                  <thead>
			                                      <tr>
			                                          <th>Sản phẩm</th>
			                                          <th>Số lượng</th>
			                                          <th>Tổng tiền</th>
			                                      </tr>
			                                  </thead>
			                                  <tbody>
			                                      {{string.Join("", order.Items.Select(item =>
													  $"<tr><td>{item.ProductName}</td>" +
													  $"<td>{item.Quantity}</td>" +
													  $"<td>{item.TotalPrice:N0} VNĐ</td></tr>"))}}
			                                  </tbody>
			                              </table>
			                              <p>Hãy đưa mã QR này cho nhân viên khi bạn check-in tại rạp:</p>
			                  
			                              <div class='qr-code'>
			                                  <div style='display: flex; justify-content: center; align-items: center;'>
			                                      <div style='text-align: center;'>
			                                          <p>Vé lấy đồ ăn:</p>
			                                          <img src='{{orderUrl}}' alt='QR Code' width='200' height='200' />
			                                      </div>
			                                  </div>
			                              </div>
			                          </div>
			                          <div class='footer'>
			                              <p>Địa chỉ: Tòa nhà xxx, Công Viên Phần Mềm Quang Trung, Tầng 3</p>
			                          </div>
			                      </div>
			                  </body>
			                  </html>
			                  """;
			return emailBody;
		}

		private static string GetBookingAndOrderTemplate(string email, string phone, BookingDetailDto booking, OrderDetailDto order)
		{
			var orderData = new
			{
				orderId = order.Id
			};
			var orderJson = JsonSerializer.Serialize(orderData);
			var orderUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(orderJson)}&size=200x200";
			var bookingData = new
			{
				bookingId = booking.Id,
				seatIds = booking.Seats.Select(bs => bs.SeatId).ToList()
			};
			var bookingJson = JsonSerializer.Serialize(bookingData);
			var bookingUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(bookingJson)}&size=200x200";
			var mailBody = $$"""
			                 <!DOCTYPE html>
			                 <html lang='vi'>
			                 <head>
			                    <meta charset='UTF-8'>
			                    <title>Thông tin đặt vé và đơn hàng</title>
			                    <style>
			                        body {
			                            font-family: Arial, sans-serif;
			                            background-color: #f4f4f4;
			                            color: #333;
			                        }
			                        .container {
			                            width: 600px;
			                            margin: auto;
			                            background-color: white;
			                            border-radius: 10px;
			                            overflow: hidden;
			                            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
			                        }
			                        .header {
			                            background-color: #007bff;
			                            color: white;
			                            padding: 20px;
			                            text-align: center;
			                        }
			                        .content {
			                            padding: 20px;
			                            line-height: 1.6;
			                            margin-bottom: 20px;
			                        }
			                        .qr-code {
			                            text-align: center;
			                            margin-top: 20px;
			                        }
			                        table {
			                            width: 100%;
			                            border-collapse: collapse;
			                            margin-top: 15px;
			                        }
			                        table, th, td {
			                            border: 1px solid #ddd;
			                        }
			                        th, td {
			                            padding: 8px;
			                            text-align: left;
			                        }
			                        th {
			                            background-color: #f8f8f8;
			                        }
			                        .footer {
			                            background-color: #007bff;
			                            color: white;
			                            text-align: center;
			                            padding: 10px;
			                        }
			                    </style>
			                 </head>
			                 <body>
			                    <div class='container'>
			                        <div class='header'>
			                            <h2>Thông tin đặt vé và đơn hàng 🎬</h2>
			                        </div>
			                        <div class='content'>
			                            <p><strong>Email:</strong> {{email}}</p>
			                            <p><strong>Số điện thoại:</strong> {{phone}}</p>
			                            <p><strong>Phim:</strong> {{booking.MovieTitle}}</p>
			                            <p><strong>Rạp:</strong> {{booking.HallName}}</p>
			                            <p><strong>Thời gian chiếu:</strong> {{booking.ShowStart}}</p>
			                            <p><strong>Tổng tiền thanh toán:</strong> {{booking.Amount + order.Amount:N0}} VNĐ</p>
			                 
			                            <h3>Thông tin vé xem phim</h3>
			                            <table>
			                                <thead>
			                                    <tr>
			                                        <th>Vị trí ghế</th>
			                                        <th>Loại ghế</th>
			                                    </tr>
			                                </thead>
			                                <tbody>
			                                    {{string.Join("", booking.Seats.Select(s =>
													$"<tr><td>{s.SeatPosition}</td>" +
													$"<td>{s.SeatTypeName}</td></tr>"))}}
			                                </tbody>
			                            </table>
			                 
			                            <h3>Thông tin đơn hàng</h3>
			                            <table>
			                                <thead>
			                                    <tr>
			                                        <th>Sản phẩm</th>
			                                        <th>Số lượng</th>
			                                        <th>Tổng tiền</th>
			                                    </tr>
			                                </thead>
			                                <tbody>
			                                 {{string.Join("", order.Items.Select(item =>
													$"<tr><td>{item.ProductName}</td>" +
													$"<td>{item.Quantity}</td>" +
													$"<td>{item.TotalPrice:N0} VNĐ</td></tr>"))}}
			                                </tbody>
			                            </table>
			                 
			                            <div class='qr-code'>
			                                <div style='display: flex; justify-content: center; align-items: center;'>
			                                    <div style='text-align: center;'>
			                                        <p>Vé xem phim:</p>
			                                        <img src='{{bookingUrl}}' alt='QR Code' width='200' height='200' />
			                                    </div>
			                                    <div style='text-align: center; margin-left: 20px;'>
			                                        <p>Vé lấy đồ ăn:</p>
			                                        <img src='{{orderUrl}}' alt='QR Code' width='200' height='200' />
			                                    </div>
			                                </div>
			                            </div>
			                        </div>
			                        <div class='footer'>
			                            <p>Địa chỉ: Tòa nhà xxx, Công Viên Phần Mềm Quang Trung, Tầng 3</p>
			                        </div>
			                    </div>
			                 </body>
			                 </html>
			                 """;
			return mailBody;
		}
	}
}