using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Delete
{
	public  sealed class DeleteFeatureTypeCommandHandler : ICommandHandler<DeleteFeatureTypeCommand, ApiResult<object>>
	{
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteFeatureTypeCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteFeatureTypeCommandHandler(
			IFeatureTypeRepository featureTypeRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<DeleteFeatureTypeCommandHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_featureTypeRepository = featureTypeRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(DeleteFeatureTypeCommand request, CancellationToken cancellationToken)
		{
			var featureTypeId = request.FeatureTypeId;
			try
			{
				var featureType = await _featureTypeRepository.GetByIdAsync(featureTypeId);
				if (featureType is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (!_featureTypeRepository.Delete(featureType))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
				}

				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delete featureType");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}