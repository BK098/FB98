using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.Application.ShowManagement.GetAll
{
	internal sealed class GetAllShowQueryHandler : IQueryHandler<GetAllShowQuery, ApiResult<PaginatedResult<GetAllShowResponse>>>
	{
		private readonly List<string> _allowedProperties = ["StartTime, MovieTitle"];
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllShowQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IShowRepository _showRepository;

		public GetAllShowQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllShowQueryHandler> logger,
			IMapper mapper,
			IShowRepository showRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_showRepository = showRepository;
		}

		public async Task<ApiResult<PaginatedResult<GetAllShowResponse>>> Handle(GetAllShowQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var today = DateTime.Today.ToUniversalTime();
				var endDate = today.AddDays(3).ToUniversalTime();

				var shows = _showRepository.GetAll()
					.Where(x => x.StartTime.Date >= today &&
								x.StartTime.Date < endDate);

				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					shows = shows.Where(x => EF.Functions.Unaccent(x.MovieTitle).ToLower().Trim().Contains(search));
				}

				if (!await shows.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllShowResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				shows = shows.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);
				shows = shows.Include(x => x.ShowStatus);
				var paginatedResult = await PaginatedResult<Show>.CreateAsync(
					shows,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var groupedShows = paginatedResult.Items
					.GroupBy(x => new { x.MovieId, x.CinemaHallId }) // Nhóm theo movieId và cinemaHallId
					.Select(group => new GetAllShowResponse
					{
						MovieId = group.Key.MovieId,
						MovieTitle = group.First().MovieTitle,
						MovieRuntimeMinutes = group.First().MovieRuntimeMinutes,
						CinemaHallId = group.Key.CinemaHallId,
						CinemaHallName = group.First().CinemaHallName,
						Dates = group
							.GroupBy(x => x.StartTime.ToString("dd-MM"))
							.Select(dateGroup => new GetAllShowByDateDto
							{
								Date = dateGroup.Key,
								ShowTimes = dateGroup.Select(x => new GetAllShowDto
								{
									ShowId = x.Id,
									StartTime = x.StartTime.ConvertUtcToVietnamTime().ToString("HH:mm:ss"),
									EndTime = x.EndTime.ConvertUtcToVietnamTime().ToString("HH:mm:ss"),
									ShowStatusId = x.ShowStatusId,
									ShowStatusName = x.ShowStatus.Name,
									IsActive = x.StartTime > DateTime.UtcNow
								}).OrderBy(x => x.StartTime).ToList()
							}).OrderBy(x => x.Date).ToList()
					}).ToList();

				var response = new PaginatedResult<GetAllShowResponse>(
					groupedShows,
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all shows");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllShowResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}