using FB98.Modules.Movies.Application.Abstractions;

namespace FB98.Modules.Movies.Application.DirectorManagement.Delete
{
	internal sealed class DeleteDirectorCommandHandler : ICommandHandler<DeleteDirectorCommand, ApiResult<object>>
	{
		private readonly IDirectorRepository _directorRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteDirectorCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteDirectorCommandHandler(
			IDirectorRepository directorRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<DeleteDirectorCommandHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_directorRepository = directorRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(DeleteDirectorCommand request, CancellationToken cancellationToken)
		{
			var directorId = request.DirectorId;
			try
			{
				var director = await _directorRepository.GetByIdAsync(directorId);
				if (director is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_directorRepository.Delete(director);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting director");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delete director");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}