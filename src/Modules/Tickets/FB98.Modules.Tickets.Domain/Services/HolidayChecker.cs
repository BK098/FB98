using Newtonsoft.Json;
using System.Globalization;

namespace FB98.Modules.Tickets.Domain.Services
{
	public class HolidayChecker
	{
		private readonly List<Holiday> _holidays;

		public HolidayChecker()
		{
			var jsonData = File.ReadAllText("SeedData/Tickets/Holidays.json");
			_holidays = JsonConvert.DeserializeObject<List<Holiday>>(jsonData)!;
		}

		public bool IsHoliday(DateTime date)
		{
			foreach (var holiday in _holidays)
			{
				if (holiday.Lunar)
				{
					if (IsLunarHoliday(date, holiday))
					{
						return true;
					}
				}
				else
				{
					if (date.ToString("dd-MM") == holiday.Date)
					{
						return true;
					}
				}
			}

			return false;
		}

		private bool IsLunarHoliday(DateTime date, Holiday holiday)
		{
			var lunarCalendar = new ChineseLunisolarCalendar();
			var lunarDay = lunarCalendar.GetDayOfMonth(date);
			var lunarMonth = lunarCalendar.GetMonth(date);
			var lunarYear = lunarCalendar.GetYear(date);

			var holidayLunarDate = DateTime.ParseExact($"{holiday.Date}-{lunarYear}", "dd-MM-yyyy", CultureInfo.InvariantCulture);

			for (var i = 0; i < holiday.Days; i++)
			{
				var currentHoliday = holidayLunarDate.AddDays(i);
				if (currentHoliday.Day == lunarDay && currentHoliday.Month == lunarMonth)
				{
					return true;
				}
			}

			return false;
		}
	}

	public class Holiday
	{
		public string Name { get; set; }
		public string Date { get; set; }
		public bool Lunar { get; set; }
		public int Days { get; set; }
	}
}