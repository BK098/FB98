using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Payments.VnPay;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	internal sealed class CreateVnPayPaymentCommandHandler : ICommandHandler<CreateVnPayPaymentCommand, ApiResult<string>>
	{
		private readonly ILogger<CreateVnPayPaymentCommandHandler> _logger;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IVnPayService _vnPayService;

		public CreateVnPayPaymentCommandHandler(
			IVnPayService vnPayService,
			ILogger<CreateVnPayPaymentCommandHandler> logger,
			IPaymentRepository paymentRepository)
		{
			_vnPayService = vnPayService;
			_logger = logger;
			_paymentRepository = paymentRepository;
		}

		public async Task<ApiResult<string>> Handle(CreateVnPayPaymentCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var transaction = new PaymentTransaction
				{
					OrderId = model.OrderId,
					BookingId = model.BookingId,
					Amount = model.Amount,
					PaymentMethodId = PaymentMethodConstants.VnPayCard,
					PaymentStatusId = PaymentStatusConstants.Peding
				};
				await _paymentRepository.CreateAsync(transaction);
				var paymentUrl = _vnPayService.GeneratePaymentUrl(model.OrderId, model.BookingId, model.Amount, model.IpAddress);
				return ApiResponseBuilder.Success(paymentUrl);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while createvnPayment");
				return ApiResponseBuilder.Error<string>("An unexpected error occurred", 500);
			}
		}
	}
}