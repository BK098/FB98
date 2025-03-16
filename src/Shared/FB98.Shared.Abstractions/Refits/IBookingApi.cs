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
	}
	public record BookingDto(Guid Id, decimal Amount, Guid StatusId);
}
