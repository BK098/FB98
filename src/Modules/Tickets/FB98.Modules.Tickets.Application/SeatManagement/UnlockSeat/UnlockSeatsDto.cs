using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Tickets.Application.SeatManagement.UnlockSeat
{
	public class UnlockSeatsDto
	{
		[SwaggerIgnore]
		public Guid? CustomerId { get; set; }
		public Guid? ShowId { get; set; }
		public ICollection<Guid>? SeatIds { get; set; } = new List<Guid>();
	}
}