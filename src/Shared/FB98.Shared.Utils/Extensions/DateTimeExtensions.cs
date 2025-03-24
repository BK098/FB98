namespace FB98.Shared.Utils.Extensions
{
	public static class DateTimeExtensions
	{
		private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		// Chuyển đổi từ UTC sang giờ Việt Nam
		public static DateTime ConvertUtcToVietnamTime(this DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
		}

		// Chuyển đổi từ giờ Việt Nam sang UTC
		public static DateTime ConvertVietnamTimeToUtc(this DateTime vietnamDateTime)
		{
			return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
		}
	}
}