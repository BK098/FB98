using FB98.Modules.Movies.Application.Abstractions;

namespace FB98.Modules.Movies.Application.CastManagement.Delete
{
	internal sealed class DeleteCastCommandHandler : ICommandHandler<DeleteCastCommand, ApiResult<object>>
	{
		private readonly ICastRepository _castRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteCastCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteCastCommandHandler(
			ICastRepository castRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<DeleteCastCommandHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_castRepository = castRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(DeleteCastCommand request, CancellationToken cancellationToken)
		{
			var castId = request.CastId;
			try
			{
				var cast = await _castRepository.GetByIdAsync(castId);
				if (cast is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_castRepository.Delete(cast);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting cast");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delete cast");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}