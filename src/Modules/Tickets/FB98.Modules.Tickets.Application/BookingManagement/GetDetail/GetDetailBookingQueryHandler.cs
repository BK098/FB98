using AutoMapper;
using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.StatusConstants;

namespace FB98.Modules.Tickets.Application.BookingManagement.GetDetail
{
	public  sealed class GetDetailBookingQueryHandler : IQueryHandler<GetDetailBookingQuery, ApiResult<GetDetailBookingResponse>>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailBookingQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailBookingQueryHandler(
			IBookingRepository bookingRepository,
			IMapper mapper,
			ILogger<GetDetailBookingQueryHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_bookingRepository = bookingRepository;
			_mapper = mapper;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<GetDetailBookingResponse>> Handle(GetDetailBookingQuery request, CancellationToken cancellationToken)
		{
			var bookingId = request.BookingId;
			try
			{
				var booking = await _bookingRepository.GetByIdAsync(bookingId);
				if (booking is null)
				{
					return ApiResponseBuilder.Error<GetDetailBookingResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailBookingResponse>(booking);
				response.StatusName = BookingStatusConstants.GetStatusName(booking.StatusId);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail booking");
				return ApiResponseBuilder.Error<GetDetailBookingResponse>("An unexpected error occurred", 500);
			}
		}
	}
}