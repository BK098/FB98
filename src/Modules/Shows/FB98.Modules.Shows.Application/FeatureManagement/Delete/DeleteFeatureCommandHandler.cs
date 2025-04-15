using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.FeatureManagement.Delete
{
	public  sealed class DeleteFeatureCommandHandler : ICommandHandler<DeleteFeatureCommand, ApiResult<object>>
	{
		private readonly IFeatureRepository _featureRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteFeatureCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteFeatureCommandHandler(
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<DeleteFeatureCommandHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_featureRepository = featureRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
		{
			var featureId = request.FeatureId;
			try
			{
				var feature = await _featureRepository.GetByIdAsync(featureId);
				if (feature is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (!_featureRepository.Delete(feature))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"), 400);
				}
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delete feature");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}