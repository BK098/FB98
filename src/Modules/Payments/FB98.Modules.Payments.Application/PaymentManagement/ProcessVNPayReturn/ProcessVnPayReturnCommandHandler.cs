using FB98.Modules.Payments.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Email;
using FB98.Shared.Infrastructure.Payments.VnPay;
using FB98.Shared.Infrastructure.SignalRHub;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using SkiaSharp;
using SkiaSharp.QrCode;

namespace FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn
{
	internal sealed class ProcessVnPayReturnCommandHandler : ICommandHandler<ProcessVnPayReturnCommand, ApiResult<string>>
	{
		private readonly IBookingApi _bookingApi;
		private readonly IBus _bus;
		private readonly IConfiguration _configuration;
		private readonly ICouponRepository _couponRepository;
		private readonly IEmailSender _emailSender;
		private readonly IHubContext<SeatHub> _hubContext;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<ProcessVnPayReturnCommandHandler> _logger;
		private readonly IOrderApi _orderApi;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IVnPayService _vnPayService;

		public ProcessVnPayReturnCommandHandler(
			IVnPayService vnPayService,
			IPaymentRepository paymentRepository,
			IBus bus,
			ILogger<ProcessVnPayReturnCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IHubContext<SeatHub> hubContext,
			IBookingApi bookingApi,
			IEmailSender emailSender,
			IOrderApi orderApi,
			IConfiguration configuration,
			ICouponRepository couponRepository)
		{
			_vnPayService = vnPayService;
			_paymentRepository = paymentRepository;
			_bus = bus;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_hubContext = hubContext;
			_bookingApi = bookingApi;
			_emailSender = emailSender;
			_orderApi = orderApi;
			_configuration = configuration;
			_couponRepository = couponRepository;
		}

		public async Task<ApiResult<string>> Handle(ProcessVnPayReturnCommand request, CancellationToken cancellationToken)
		{
			var model = request.QueryParams;
			const string suscessCode = "00";
			try
			{
				var txnRef = model["vnp_TxnRef"];
				var responseCode = model["vnp_ResponseCode"];
				var amount = decimal.Parse(model["vnp_Amount"]);

				var transaction = await _paymentRepository.GetByIdAsync(Guid.Parse(txnRef));
				if (transaction == null)
				{
					return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (transaction.PaymentStatusId != PaymentStatusConstants.Pending)
				{
					return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("PaymentProcessed"));
				}

				if (!_vnPayService.ValidateVnPayResponse(request.QueryParams, transaction.Amount, txnRef))
				{
					return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("Invalid"));
				}

				if (responseCode == suscessCode)
				{
					transaction.MarkSuccess(txnRef);
					_paymentRepository.Update(transaction);

					await _bus.Publish(new PaymentSuccessEvent(transaction.OrderId, transaction.BookingId, transaction.UserId, amount, transaction.Email), cancellationToken);

					BookingDetailDto? booking = null;
					OrderDetailDto? order = null;

					if (transaction.BookingId != null)
					{
						try
						{
							var bookingResponse = await _bookingApi.GetDetailBooking(transaction.BookingId!.Value);
							if (bookingResponse.IsSuccess)
							{
								booking = bookingResponse.Data;
							}

							await _hubContext.Clients.All.SendAsync("SeatsStatusChanged", bookingResponse.Data!.ShowId, cancellationToken);
						}
						catch (ApiException ex)
						{
							Console.WriteLine(ex);
						}
					}

					if (transaction.OrderId != null)
					{
						try
						{
							var orderResponse = await _orderApi.GetOrderDetailById(transaction.OrderId!.Value);
							if (orderResponse.IsSuccess)
							{
								order = orderResponse.Data;
							}
						}
						catch (ApiException ex)
						{
							Console.WriteLine(ex);
						}
					}

					if (!string.IsNullOrWhiteSpace(transaction.CouponCode))
					{
						await _couponRepository.ApplyCouponAfterPaymentAsync(
							transaction.CouponCode,
							transaction.Id,
							transaction.Amount);
					}

					await SendMailAsync(transaction.Email, transaction.PhoneNumber, booking, order);

					return ApiResponseBuilder.Success(transaction.Id.ToString(), _localizedMessageService.GetLocalizedMessage("PaymentSuccessful"));
				}

				transaction.MarkFailed();
				_paymentRepository.Update(transaction);
				await _bus.Publish(new PaymentFailedEvent(transaction.OrderId, transaction.BookingId, "Payment failed."), cancellationToken);

				return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("PaymentFailed"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing payment");
				return ApiResponseBuilder.Error<string>("An unexpected error occurred", 500);
			}
		}

		private async Task SendMailAsync(string email, string phoneNumber, BookingDetailDto? booking, OrderDetailDto? order)
		{
			byte[]? qrBooking = null;
			byte[]? qrOrder = null;
			var baseUrl = _configuration["BackEndBaseUrl"];
			if (booking != null)
			{
				qrBooking = GenerateQrCode(new
				{
					bookingId = booking.Id,
					seatIds = booking.Seats.Select(bs => bs.SeatId).ToList()
				}, $"{baseUrl}/ticket-module/bookings/check-in");
			}

			if (order != null)
			{
				qrOrder = GenerateQrCode(new
				{
					orderId = order.Id
				}, $"{baseUrl}/order-module/orders/check-in");
			}

			var emailBody = booking != null && order != null
				? GetBookingAndOrderTemplate(email, phoneNumber, booking, order)
				: booking != null
					? GetBookingOnlyTemplate(email, phoneNumber, booking)
					: GetOrderOnlyTemplate(email, phoneNumber, order!);

			var attachments = new List<(byte[], string)>();
			if (qrBooking != null)
			{
				attachments.Add((qrBooking, "BookingQR.png"));
			}

			if (qrOrder != null)
			{
				attachments.Add((qrOrder, "OrderQR.png"));
			}

			await _emailSender.SendEmailWithMultipleInlineAttachmentsAsync(
				email,
				"Xác nhận đơn hàng của bạn",
				emailBody,
				attachments);
		}

		private static byte[] GenerateQrCode(object qrDataObject, string urlEndpoint)
		{
			var qrCurl = $"""
			              curl -X 'POST' \
			              '{urlEndpoint}' \
			              -H 'accept: */*' \
			              -H 'Content-Type: application/json-patch+json' \
			              -d '{JsonConvert.SerializeObject(qrDataObject, Formatting.Indented)}'
			              """;

			var qrCodeData = new QRCodeGenerator().CreateQrCode(qrCurl, ECCLevel.L);
			const int size = 300;
			var moduleSize = size / qrCodeData.ModuleMatrix.Count;

			using var bitmap = new SKBitmap(size, size);
			using var canvas = new SKCanvas(bitmap);
			canvas.Clear(SKColors.White);

			for (var row = 0; row < qrCodeData.ModuleMatrix.Count; row++)
				for (var col = 0; col < qrCodeData.ModuleMatrix[row].Count; col++)
				{
					var color = qrCodeData.ModuleMatrix[row][col] ? SKColors.Black : SKColors.White;
					canvas.DrawRect(col * moduleSize, row * moduleSize, moduleSize, moduleSize, new SKPaint { Color = color });
				}

			using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
			using var stream = new MemoryStream();
			data.SaveTo(stream);
			return stream.ToArray();
		}

		private static string GetBookingOnlyTemplate(string email, string phone, BookingDetailDto booking)
		{
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
			                                         <img src="cid:BookingQR.png" alt='QR Code' width='200' height='200' />
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
			                                          <img src='cid:OrderQR.png' alt='QR Code' width='200' height='200' />
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
			                                        <img src='cid:BookingQR.png' alt='QR Code' width='200' height='200' />
			                                    </div>
			                                    <div style='text-align: center; margin-left: 20px;'>
			                                        <p>Vé lấy đồ ăn:</p>
			                                        <img src='cid:OrderQR.png' alt='QR Code' width='200' height='200' />
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