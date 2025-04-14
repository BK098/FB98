using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;

namespace FB98.Modules.Payments.Application.CouponManagement.Create
{
	internal sealed class CreateCouponCommandHandler : ICommandHandler<CreateCouponCommand, ApiResult<object>>
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCouponCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateCouponDto> _validator;

		public CreateCouponCommandHandler(
			ILogger<CreateCouponCommandHandler> logger,
			ICouponRepository couponRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			IValidator<CreateCouponDto> validator)
		{
			_logger = logger;
			_couponRepository = couponRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var normalizedCode = model.Code!.Normalize().ToUpper().Trim();
				if (!string.IsNullOrWhiteSpace(model.Code))
				{
					if (!normalizedCode.Equals(model.Code, StringComparison.Ordinal))
					{
						if (await _couponRepository.IsCouponExisted(normalizedCode))
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
						}

						model.Code = normalizedCode;
					}
				}

				var coupon = _mapper.Map<Coupon>(model);
				coupon.Code = model.Code;
				coupon.StartDate = model.StartDate!.Value.ToUniversalTime();
				coupon.EndDate = model.EndDate!.Value.ToUniversalTime();

				await _couponRepository.CreateAsync(coupon);

				return ApiResponseBuilder.Success<object>(coupon.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create coupon");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}