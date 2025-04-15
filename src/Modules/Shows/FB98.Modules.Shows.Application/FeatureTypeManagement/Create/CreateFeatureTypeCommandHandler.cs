using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Create
{
	public  sealed class CreateFeatureTypeCommandHandler : ICommandHandler<CreateFeatureTypeCommand, ApiResult<object>>
	{
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateFeatureTypeCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateFeatureTypeDto> _validator;

		public CreateFeatureTypeCommandHandler(
			IFeatureTypeRepository featureTypeRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateFeatureTypeCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<CreateFeatureTypeDto> validator)
		{
			_featureTypeRepository = featureTypeRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(CreateFeatureTypeCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var featureTypeExisted = await _featureTypeRepository.IsFeatureTypeExistsAsync(model.Name!, cancellationToken);
				if (featureTypeExisted)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				var featureType = _mapper.Map<FeatureType>(model);
				await _featureTypeRepository.CreateAsync(featureType);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(featureType.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create featureType");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}