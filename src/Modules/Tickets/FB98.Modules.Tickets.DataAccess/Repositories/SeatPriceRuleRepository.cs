using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Modules.Tickets.Domain.Services;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	internal class SeatPriceRuleRepository : BaseRepository<SeatPriceRule, TicketModuleDbContext>, ISeatPriceRuleRepository
	{
		private readonly HolidayChecker _holidayChecker;

		public SeatPriceRuleRepository(
			TicketModuleDbContext context,
			HolidayChecker holidayChecker)
			: base(context)
		{
			_holidayChecker = holidayChecker;
		}

		public async Task<SeatPriceRule?> GetSeatPriceByTypeAndDate(Guid seatTypeId, DateTime showDate) //, CustomerTypeEnum customerType)
		{
			var dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;

			var isHoliday = _holidayChecker.IsHoliday(showDate);

			var seatPriceRule = await _context.SeatPriceRules
				.Where(rule =>
					rule.SeatTypeId == seatTypeId &&
					rule.IsActived && (
					rule.StartDate <= showDate ||
					rule.EndDate >= showDate ||
					rule.DaysOfWeek == dayOfWeek ||
					isHoliday == rule.IsHoliday)
				).OrderByDescending(rule => rule.IsDefault)
				.FirstOrDefaultAsync();

			return seatPriceRule;
		}
	}
}