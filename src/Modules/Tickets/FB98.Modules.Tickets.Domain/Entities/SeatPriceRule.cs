using FB98.Modules.Tickets.Domain.Enums;
using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class SeatPriceRule : BaseEntity
	{
		public Guid SeatTypeId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int DaysOfWeek { get; set; }
		public int MinAge { get; set; }
		public int MaxAge { get; set; }
		public bool IsDefault { get; set; }
		public bool IsActived { get; set; }
		public CustomerTypeEnum CustomerType { get; set; }

		public bool IsValid()
		{
			var now = DateTime.UtcNow;
			return StartDate <= now && EndDate >= now;
		}
	}
}