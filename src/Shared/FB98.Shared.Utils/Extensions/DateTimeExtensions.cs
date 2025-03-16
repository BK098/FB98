namespace FB98.Shared.Utils.Extensions
{
	public static class DateTimeExtensions
	{
		private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		// Extension method để chuyển đổi từ UTC sang giờ Việt Nam
		public static DateTime ConvertUtcToVietnamTime(this DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
		}

		// Extension method để chuyển đổi từ giờ Việt Nam sang UTC
		public static DateTime ConvertVietnamTimeToUtc(this DateTime vietnamDateTime)
		{
			return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
		}
	}
}