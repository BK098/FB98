using AutoMapper;
using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.Application.BookingManagement.GetAll
{
	public  sealed class GetAllBookingQueryHandler : IQueryHandler<GetAllBookingQuery, ApiResult<PaginatedResult<GetAllBookingResponse>>>
	{
		private readonly List<string> _allowedProperties = ["UserPhone, Amount, ShowStart"];
		private readonly IBookingRepository _bookingRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllBookingQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllBookingQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllBookingQueryHandler> logger,
			IMapper mapper,
			IBookingRepository bookingRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_bookingRepository = bookingRepository;
		}

		public async Task<ApiResult<PaginatedResult<GetAllBookingResponse>>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var entities = _bookingRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x =>
						EF.Functions.Unaccent(x.UserPhone).ToLower().Trim().Contains(search) ||
						EF.Functions.Unaccent(x.UserName).ToLower().Trim().Contains(search) ||
						EF.Functions.Unaccent(x.UserId.ToString()).ToLower().Trim().Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllBookingResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending).OrderBy(x => x.UpdateAt);

				var paginatedResult = await PaginatedResult<Booking>.CreateAsync(
					entities,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllBookingResponse>(
					_mapper.Map<List<GetAllBookingResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all booking");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllBookingResponse>>("An unexpected error occurred",
					500);
			}
		}
	}
}