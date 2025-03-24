using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	public class SeatReservationDto
	{
		[SwaggerIgnore]
		public Guid? UserId { get; set; }

		[SwaggerIgnore]
		public string? UserName { get; set; }

		[SwaggerIgnore]
		public string? UserPhone { get; set; }

		public Guid? ShowId { get; set; }
	}
}