using AutoMapper;
using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.SeatPriceRules.Create
{
	internal sealed class CreateRuleCommandHandler : ICommandHandler<CreateRuleCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateRuleCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ISeatPriceRuleRepository _seatPriceRuleRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateRuleDto> _validator;

		public CreateRuleCommandHandler(
			ISeatPriceRuleRepository seatPriceRuleRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper,
			IValidator<CreateRuleDto> validator,
			IUnitOfWork unitOfWork,
			ILogger<CreateRuleCommandHandler> logger)
		{
			_seatPriceRuleRepository = seatPriceRuleRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
			_validator = validator;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<ApiResult<object>> Handle(CreateRuleCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var seatPriceRule = _mapper.Map<SeatPriceRule>(model);
				await _seatPriceRuleRepository.CreateAsync(seatPriceRule);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(seatPriceRule.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create new discount rule");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}