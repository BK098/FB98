using FB98.Modules.Movies.Application.Abstractions;
using FB98.Shared.Abstractions.Refits;
using Refit;
using System.Net;

namespace FB98.Modules.Movies.Application.MovieManagement.Delete
{
	internal sealed class DeleteMovieCommandHandler : ICommandHandler<DeleteMovieCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteMovieCommandHandler> _logger;
		private readonly IMovieRepository _movieRepository;
		private readonly IShowApi _showApi;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteMovieCommandHandler(
			IMovieRepository movieRepository,
			IUnitOfWork unitOfWork,
			ILogger<DeleteMovieCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IShowApi showApi)
		{
			_movieRepository = movieRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_showApi = showApi;
		}

		public async Task<ApiResult<object>> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
		{
			var movieId = request.MovieId;
			try
			{
				var movie = await _movieRepository.GetByIdAsync(movieId);
				if (movie is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				try
				{
					var showResult = await _showApi.IsMovieInAnyShow(movieId);
					if (showResult.Data)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("MovieInUseInShows"));
					}
				}
				catch (ApiException e)
				{
					if (e.StatusCode == HttpStatusCode.NotFound)
					{
						_logger.LogWarning(e, "Show API not found");
					}
					else
					{
						_logger.LogError(e, "Error occurred while checking if movie is in any show");
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SOS"));
					}
				}

				_movieRepository.Delete(movie);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting movie");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delete movie");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}