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
		public int? DaysOfWeek { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int? MinAge { get; set; }
		public int? MaxAge { get; set; }
		public CustomerTypeEnum CustomerType { get; set; } = CustomerTypeEnum.Default;
		public bool IsDefault { get; set; } = false;
		public bool IsActived { get; set; } = false;
		public bool IsHoliday { get; set; } = false;
	}
}