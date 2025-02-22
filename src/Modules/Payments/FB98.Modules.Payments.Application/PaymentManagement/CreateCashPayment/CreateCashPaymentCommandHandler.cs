using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	internal sealed class CreateCashPaymentCommandHandler : ICommandHandler<CreateCashPaymentCommand, ApiResult<object>>
	{
		private readonly ILogger<CreateCashPaymentCommandHandler> _logger;
		private readonly IPaymentRepository _paymentRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IBus _bus;

		public CreateCashPaymentCommandHandler(
			IPaymentRepository paymentRepository,
			ILogger<CreateCashPaymentCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IBus bus)
		{
			_paymentRepository = paymentRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_bus = bus;
		}

		public async Task<ApiResult<object>> Handle(CreateCashPaymentCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var payment = new PaymentTransaction
				{
					OrderId = model.OrderId,
					BookingId = model.BookingId,
					Amount = model.Amount,
					PaymentMethodId = PaymentMethodConstants.Cash,
					PaymentStatusId = PaymentStatusConstants.Success
				};
				await _paymentRepository.CreateAsync(payment);

				await _bus.Publish(new PaymentSuccessEvent(payment.OrderId!.Value, payment.BookingId), cancellationToken);
				return ApiResponseBuilder.Success<object>(payment.Id, _localizedMessageService.GetLocalizedMessage("PaymentSuccessful"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create cash payment");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}