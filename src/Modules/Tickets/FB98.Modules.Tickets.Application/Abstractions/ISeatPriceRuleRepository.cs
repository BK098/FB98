using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.Abstractions
{
	public interface ISeatPriceRuleRepository : IRepository<SeatPriceRule>
	{
		Task<SeatPriceRule?> GetSeatPriceByTypeAndDate(Guid seatTypeId, DateTime showDate);//, CustomerTypeEnum customerType);
	}
}