using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.ShowManagement.Delete
{
	internal sealed class DeleteShowCommandHandler : ICommandHandler<DeleteShowCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteShowCommandHandler> _logger;
		private readonly IShowRepository _showRepository;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteShowCommandHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<DeleteShowCommandHandler> logger,
			IShowRepository showRepository,
			IUnitOfWork unitOfWork)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_showRepository = showRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(DeleteShowCommand request, CancellationToken cancellationToken)
		{
			var showId = request.ShowId;
			try
			{
				var show = await _showRepository.GetByIdAsync(showId);
				if (show == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_showRepository.Delete(show);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting show");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting show");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}