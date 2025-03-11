using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Modules.Tickets.Domain.Enums;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	internal class SeatPriceRuleRepository : BaseRepository<SeatPriceRule, TicketModuleDbContext>, ISeatPriceRuleRepository
	{
		public SeatPriceRuleRepository(TicketModuleDbContext context) : base(context)
		{
		}

		public async Task<decimal?> GetSeatPriceByTypeAndDate(Guid seatTypeId, DateTime showDate, CustomerTypeEnum customerType)
		{
			var dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;

			// Tìm quy tắc giá phù hợp
			var seatPriceRule = await _context.SeatPriceRules
				.Where(rule =>
						rule.SeatTypeId == seatTypeId &&
						rule.CustomerType == customerType &&
						rule.StartDate <= showDate &&
						rule.EndDate >= showDate &&
						rule.IsActived &&
						rule.DaysOfWeek == dayOfWeek
				).OrderByDescending(rule => rule.IsDefault) // Ưu tiên giá mặc định
				.FirstOrDefaultAsync();

			return seatPriceRule?.Price;
		}
	}
}