using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.Application.ShowManagement.GetAllByMovieId
{
	internal sealed class GetAllShowByMovieIdQueryHandler : IQueryHandler<GetAllShowByMovieIdQuery, ApiResult<PaginatedResult<GetAllShowByMovieIdResponse>>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllShowByMovieIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IShowRepository _showRepository;

		public GetAllShowByMovieIdQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllShowByMovieIdQueryHandler> logger,
			IMapper mapper,
			IShowRepository showRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_showRepository = showRepository;
		}

		public async Task<ApiResult<PaginatedResult<GetAllShowByMovieIdResponse>>> Handle(GetAllShowByMovieIdQuery request, CancellationToken cancellationToken)
		{
			var movideId = request.MovieId;
			try
			{
				var today = DateTime.Today.ToUniversalTime();
				var endDate = today.AddDays(3).ToUniversalTime();

				var shows = _showRepository.GetAll()
					.Include(x => x.ShowStatus)
					.Where(x => x.MovieId == movideId &&
								x.StartTime.Date >= today &&
								x.StartTime.Date < endDate);

				if (!await shows.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllShowByMovieIdResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var groupedShows = shows
					.AsEnumerable()
					.GroupBy(x => new { x.StartTime.Date, x.CinemaHallId })
					.Select(group => new GetAllShowByMovieIdResponse
					{
						Date = group.Key.Date.ToString("M"),
						CinemaHallId = group.Key.CinemaHallId,
						CinemaHallName = group.First().CinemaHallName,
						ShowTimes = group.Select(show => new GetAllShowDto
						{
							StartTime = show.StartTime.ConvertUtcToVietnamTime().ToString("HH:mm:ss zz"),
							EndTime = show.EndTime.ConvertUtcToVietnamTime().ToString("HH:mm:ss zz"),
							ShowId = show.Id,
							ShowStatusId = show.ShowStatusId,
							ShowStatusName = show.ShowStatus.Name
						}).OrderBy(x => x.StartTime).ToList()
					}).OrderBy(x => x.Date).ToList();

				var response = new PaginatedResult<GetAllShowByMovieIdResponse>(
					groupedShows,
					1,
					groupedShows.Count,
					groupedShows.Count);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all shows by movieId");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllShowByMovieIdResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}