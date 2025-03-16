using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	internal class SeatPriceRuleRepository : BaseRepository<SeatPriceRule, TicketModuleDbContext>, ISeatPriceRuleRepository
	{
		public SeatPriceRuleRepository(TicketModuleDbContext context) : base(context)
		{
		}

		public async Task<SeatPriceRule?> GetSeatPriceByTypeAndDate(Guid seatTypeId, DateTime showDate)//, CustomerTypeEnum customerType)
		{
			var dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;

			var seatPriceRule = await _context.SeatPriceRules
				.Where(rule =>
					rule.SeatTypeId == seatTypeId &&
					//rule.CustomerType == customerType &&
					rule.IsActived ||
					rule.StartDate <= showDate ||
					rule.EndDate >= showDate ||
					rule.DaysOfWeek == dayOfWeek
				).OrderByDescending(rule => rule.IsDefault)
				.FirstOrDefaultAsync();

			return seatPriceRule;
		}
	}
}