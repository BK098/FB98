using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FB98.Shared.Utils.Extensions
{
	public static class StringExtension
	{
		public static string RemoveDiacritics(this string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;

			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		static Regex? ConvertToUnsign_rg;

		public static string ConvertToUnsign(this string strInput)
		{
			if (string.IsNullOrEmpty(strInput))
				return strInput;
			if (ReferenceEquals(ConvertToUnsign_rg, null))
			{
				ConvertToUnsign_rg = new Regex(@"[\p{IsCombiningDiacriticalMarks}]+", RegexOptions.Compiled);
			}

			// Chuỗi chuẩn hóa thành NFD (tách dấu)
			var temp = strInput.Normalize(NormalizationForm.FormD);

			// Loại bỏ dấu và thay thế "đ" với "d"
			return ConvertToUnsign_rg.Replace(temp, string.Empty)
				.Replace("đ", "d")
				.Replace("Đ", "D")
				.ToLower(); // Nếu bạn cần chữ thường
		}
	}
}