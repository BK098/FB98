using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.FeatureManagement.Update
{
	public  sealed class UpdateFeatureCommandHandler : ICommandHandler<UpdateFeatureCommand, ApiResult<object>>
	{
		private readonly IFeatureRepository _featureRepository;
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateFeatureCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateFeatureDto> _validator;

		public UpdateFeatureCommandHandler(
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<UpdateFeatureCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<UpdateFeatureDto> validator,
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

		public async Task<ApiResult<object>> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var featureId = request.FeatureId;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				if (await _featureTypeRepository.GetByIdAsync(model.FeatureTypeId) == null)
				{
					return ApiResponseBuilder.Error<object>("FeatureType: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var feature = await _featureRepository.GetByIdAsync(featureId);
				if (feature is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (feature.Name != model.Name && await _featureRepository.IsFeatureExistsAsync(model.Name!, cancellationToken))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				_mapper.Map(model, feature);
				_featureRepository.Update(feature);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(featureId, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update feature");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}