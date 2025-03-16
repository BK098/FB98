using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	public class SeatReservationDto
	{
		[SwaggerIgnore]
		public Guid? CustomerId { get; set; }
		public Guid? ShowId { get; set; }
	}
}