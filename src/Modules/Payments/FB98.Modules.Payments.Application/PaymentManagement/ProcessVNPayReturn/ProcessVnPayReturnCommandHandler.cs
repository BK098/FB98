using FB98.Modules.Payments.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Payments.VnPay;
using FB98.Shared.Infrastructure.SignalRHub;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Refit;

namespace FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn
{
	internal sealed class ProcessVnPayReturnCommandHandler : ICommandHandler<ProcessVnPayReturnCommand, ApiResult<string>>
	{

		private readonly IBus _bus;
		private readonly ICouponRepository _couponRepository;
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
			IOrderApi orderApi,
			ICouponRepository couponRepository)
		{
			_vnPayService = vnPayService;
			_paymentRepository = paymentRepository;
			_bus = bus;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_hubContext = hubContext;
			_orderApi = orderApi;
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
					await _bus.Publish(new PaymentSuccessEvent(transaction.OrderId, transaction.UserId, amount, transaction.Email), cancellationToken);

					OrderDetailDto? order = null;

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
						await _couponRepository.ApplyCouponAfterPaymentAsync(transaction.CouponCode, transaction.Id, transaction.Amount);
					}

					await EmailService.SendMailAsync(transaction.Email, transaction.PhoneNumber, order);

					transaction.MarkSuccess(txnRef);
					_paymentRepository.Update(transaction);
					return ApiResponseBuilder.Success(transaction.Id.ToString(), _localizedMessageService.GetLocalizedMessage("PaymentSuccessful"));
				}

				transaction.MarkFailed();
				_paymentRepository.Update(transaction);
				await _bus.Publish(new PaymentFailedEvent(transaction.OrderId, "Payment failed."), cancellationToken);

				return ApiResponseBuilder.Error<string>(_localizedMessageService.GetLocalizedMessage("PaymentFailed"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing payment");
				return ApiResponseBuilder.Error<string>("An unexpected error occurred", 500);
			}
		}
	}
}