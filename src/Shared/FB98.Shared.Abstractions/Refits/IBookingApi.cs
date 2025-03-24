using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Payments.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IBookingApi
	{
		[Get("/ticket-module/bookings/{bookingId}")]
		Task<ApiResult<BookingDto>> GetBookingById(Guid bookingId);

		[Get("/ticket-module/bookings/{bookingId}")]
		Task<ApiResult<BookingDetailDto>> GetDetailBooking(Guid bookingId);
	}
	public record BookingDto(Guid Id, decimal Amount, Guid StatusId, Guid ShowId);
	public record BookingDetailDto(Guid Id, decimal Amount, Guid ShowId, string ShowStart, string MovieTitle, string HallName, IEnumerable<BookingSeatDetailDto> Seats);
	public record BookingSeatDetailDto(Guid SeatId, string SeatPosition, Guid SeatStatusId, string SeatTypeName, decimal Price);
}
