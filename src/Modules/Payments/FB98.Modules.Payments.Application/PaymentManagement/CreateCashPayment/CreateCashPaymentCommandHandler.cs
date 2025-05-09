using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;
using Refit;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	internal sealed class CreateCashPaymentCommandHandler : ICommandHandler<CreateCashPaymentCommand, ApiResult<object>>
	{
		private readonly IBus _bus;
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCashPaymentCommandHandler> _logger;
		private readonly IOrderApi _orderApi;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IUserApi _userApi;

		public CreateCashPaymentCommandHandler(IPaymentRepository paymentRepository, ILogger<CreateCashPaymentCommandHandler> logger, ILocalizedMessageService localizedMessageService, IBus bus, IUserApi userApi, IOrderApi orderApi, ICouponRepository couponRepository)
		{
			_paymentRepository = paymentRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_bus = bus;
			_userApi = userApi;
			_orderApi = orderApi;
			_couponRepository = couponRepository;
		}

		public async Task<ApiResult<object>> Handle(CreateCashPaymentCommand request, CancellationToken cancellationToken)
		{
			var searchTerm = request.SearchTerm?.Trim();
			var model = request.Model;
			var now = DateTime.UtcNow;
			decimal amount = 0;
			try
			{
				var email = string.Empty;
				var phoneNumber = string.Empty;
				var userId = Guid.Empty;
				try
				{
					if (searchTerm != null)
					{
						var userResponse = await _userApi.GetUserProfile(new UserDto(searchTerm));

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

				var transaction = new PaymentTransaction
				{
					Email = email,
					PhoneNumber = phoneNumber,
					UserId = userId,
					OrderId = model.OrderId,
					SubAmount = amount,
					PaymentMethodId = PaymentMethodConstants.Cash
				};

				decimal discount = 0;
				if (!string.IsNullOrWhiteSpace(model.CouponCode))
				{
					var coupon = await _couponRepository.GetValidCouponAsync(model.CouponCode.Normalize().ToUpper().Trim(), amount, now);
					if (coupon == null)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("CouponInvalid"));
					}

					discount = coupon.CalculateDiscount(amount);
					transaction.CouponCode = coupon.Code;
				}

				var finalAmount = amount - discount;
				transaction.Amount = finalAmount;

				transaction.MarkSuccess();
				await _paymentRepository.CreateAsync(transaction);

				if (!string.IsNullOrWhiteSpace(transaction.CouponCode))
				{
					await _couponRepository.ApplyCouponAfterPaymentAsync(transaction.CouponCode, transaction.Id, transaction.Amount);
				}

				await EmailService.SendMailAsync(transaction.Email, transaction.PhoneNumber, order);
				await _bus.Publish(new PaymentCreatedEvent(userId, model.OrderId), cancellationToken);
				Thread.Sleep(50);
				await _bus.Publish(new PaymentSuccessEvent(transaction.OrderId, transaction.UserId, amount, transaction.Email), cancellationToken);
				return ApiResponseBuilder.Success<object>(transaction.Id, _localizedMessageService.GetLocalizedMessage("PaymentSuccessful"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create cash payment");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}