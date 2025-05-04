using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Payments.VnPay;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Refit;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	internal sealed class CreateVnPayPaymentCommandHandler : ICommandHandler<CreateVnPayPaymentCommand, ApiResult<string>>
	{
		private readonly IBookingApi _bookingApi;
		private readonly IBus _bus;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateVnPayPaymentCommandHandler> _logger;
		private readonly IOrderApi _orderApi;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IVnPayService _vnPayService;

		public CreateVnPayPaymentCommandHandler(
			IVnPayService vnPayService,
			ILogger<CreateVnPayPaymentCommandHandler> logger,
			IPaymentRepository paymentRepository,
			IBus bus,
			IHttpContextAccessor contextAccessor,
			IOrderApi orderApi,
			IBookingApi bookingApi,
			ILocalizedMessageService localizedMessageService,
			ICouponRepository couponRepository)
		{
			_vnPayService = vnPayService;
			_logger = logger;
			_paymentRepository = paymentRepository;
			_bus = bus;
			_contextAccessor = contextAccessor;
			_orderApi = orderApi;
			_bookingApi = bookingApi;
			_localizedMessageService = localizedMessageService;
			_couponRepository = couponRepository;
		}

		public async Task<ApiResult<string>> Handle(CreateVnPayPaymentCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var now = DateTime.UtcNow;
			decimal amount = 0;
			try
			{
				if (model.OrderId == null && model.BookingId == null)
				{
					return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("OrderOrBookingRequired"));
				}

				try
				{
					if (model.OrderId != null)
					{
						var orderResponse = await _orderApi.GetOrderById(model.OrderId!.Value);
						amount += orderResponse.Data!.Amount;
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<string>("Order: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				try
				{
					if (model.BookingId != null)
					{
						var bookingResponse = await _bookingApi.GetBookingById(model.BookingId!.Value);

						amount += bookingResponse.Data!.Amount;
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<string>("Booking: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var transaction = new PaymentTransaction
				{
					Email = model.Email!,
					PhoneNumber = model.PhoneNumber!,
					UserId = model.UserId!.Value,
					OrderId = model.OrderId,
					BookingId = model.BookingId,
					SubAmount = amount,
					PaymentMethodId = PaymentMethodConstants.VnPayCard
				};

				decimal discount = 0;
				if (!string.IsNullOrWhiteSpace(model.CouponCode))
				{
					var coupon = await _couponRepository.GetValidCouponAsync(model.CouponCode.Normalize().ToUpper().Trim(), amount, now);
					if (coupon == null)
					{
						return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("CouponInvalid"));
					}

					discount = coupon.CalculateDiscount(amount);
					transaction.CouponCode = coupon.Code;
				}

				var finalAmount = amount - discount;

				transaction.Amount = finalAmount;
				transaction.MarkPeding();

				var ipAddress = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
				await _paymentRepository.CreateAsync(transaction);
				var paymentUrl = _vnPayService.GeneratePaymentUrl(transaction.Id, transaction.Amount, ipAddress);

				await _bus.Publish(new VnPayPaymentCreatedEvent(model.UserId!.Value, model.BookingId, model.OrderId), cancellationToken);

				return ApiResponseBuilder.Success(paymentUrl);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while createVnPayment");
				return ApiResponseBuilder.Error<string>("An unexpected error occurred", 500);
			}
		}
	}
}