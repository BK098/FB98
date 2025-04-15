using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;

namespace FB98.Modules.Shows.Application.FeatureManagement.Create
{
	public  sealed class CreateFeatureCommandHandler : ICommandHandler<CreateFeatureCommand, ApiResult<object>>
	{
		private readonly IFeatureRepository _featureRepository;
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateFeatureCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateFeatureDto> _validator;

		public CreateFeatureCommandHandler(
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateFeatureCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<CreateFeatureDto> validator,
			IFeatureTypeRepository featureTypeRepository)
		{
			_featureRepository = featureRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_featureTypeRepository = featureTypeRepository;
		}

		public async Task<ApiResult<object>> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var featureExisted = await _featureRepository.IsFeatureExistsAsync(model.Name!, cancellationToken);
				if (featureExisted)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				if (await _featureTypeRepository.GetByIdAsync(model.FeatureTypeId) == null)
				{
					return ApiResponseBuilder.Error<object>("FeatureType: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var feature = _mapper.Map<Feature>(model);
				await _featureRepository.CreateAsync(feature);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(feature.Id , _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create feature");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}