using FB98.Modules.Payments.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Infrastructure.Payments.VnPay;
using MassTransit;

namespace FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn
{
	internal sealed class ProcessVnPayReturnCommandHandler : ICommandHandler<ProcessVnPayReturnCommand, ApiResult<string>>
	{
		private readonly IBus _bus;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<ProcessVnPayReturnCommandHandler> _logger;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IVnPayService _vnPayService;

		public ProcessVnPayReturnCommandHandler(
			IVnPayService vnPayService,
			IPaymentRepository paymentRepository,
			IBus bus,
			ILogger<ProcessVnPayReturnCommandHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_vnPayService = vnPayService;
			_paymentRepository = paymentRepository;
			_bus = bus;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<string>> Handle(ProcessVnPayReturnCommand request, CancellationToken cancellationToken)
		{
			var model = request.QueryParams;
			try
			{
				var txnRef = model["vnp_TxnRef"];
				var responseCode = model["vnp_ResponseCode"];
				var transationId = model.ContainsKey("vnp_TransactionNo");

				var transaction = await _paymentRepository.GetByIdAsync(Guid.Parse(txnRef));
				if (transaction == null)
				{
					return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (_vnPayService.ValidateVnPayResponse(request.QueryParams))
				{
					return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("Invalid"));
				}

				if (responseCode == "00")
				{
					transaction.MarkSuccess();
					_paymentRepository.Update(transaction);
					await _bus.Publish(new PaymentSuccessEvent(transaction.OrderId!.Value, transaction.BookingId), cancellationToken);
					return ApiResponseBuilder.Success(_localizedMessageService.GetLocalizedMessage("PaymentSuccessful"));
				}

				transaction.MarkFailed();
				_paymentRepository.Update(transaction);
				await _bus.Publish(new PaymentFailedEvent(transaction.OrderId!.Value, transaction.BookingId, "Payment failed."), cancellationToken);

				return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("PaymentFailed"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while proccess payment");
				return ApiResponseBuilder.Error<string>("An unexpected error occurred", 500);
			}
		}
	}
}