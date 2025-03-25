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
			ILocalizedMessageService localizedMessageService)
		{
			_vnPayService = vnPayService;
			_logger = logger;
			_paymentRepository = paymentRepository;
			_bus = bus;
			_contextAccessor = contextAccessor;
			_orderApi = orderApi;
			_bookingApi = bookingApi;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<string>> Handle(CreateVnPayPaymentCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			decimal amount = 0;
			try
			{
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
					Amount = amount,
					PaymentMethodId = PaymentMethodConstants.VnPayCard
				};
				transaction.MarkPeding();

				var ipAddress = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
				await _paymentRepository.CreateAsync(transaction);
				var paymentUrl = _vnPayService.GeneratePaymentUrl(transaction.Id, amount, ipAddress);

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