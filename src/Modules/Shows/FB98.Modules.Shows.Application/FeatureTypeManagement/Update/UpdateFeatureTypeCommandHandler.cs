using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Update
{
	public  sealed class UpdateFeatureTypeCommandHandler : ICommandHandler<UpdateFeatureTypeCommand, ApiResult<object>>
	{
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateFeatureTypeCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateFeatureTypeDto> _validator;

		public UpdateFeatureTypeCommandHandler(
			IFeatureTypeRepository featureTypeRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<UpdateFeatureTypeCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<UpdateFeatureTypeDto> validator)
		{
			_featureTypeRepository = featureTypeRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UpdateFeatureTypeCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var featureTypeId = request.FeatureTypeId;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var featureType = await _featureTypeRepository.GetByIdAsync(featureTypeId);
				if (featureType is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (featureType.Name != model.Name)
				{
					if (await _featureTypeRepository.IsFeatureTypeExistsAsync(model.Name!, cancellationToken))
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
					}
				}

				_mapper.Map(model, featureType);
				_featureTypeRepository.Update(featureType);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(featureTypeId, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update feature type");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}