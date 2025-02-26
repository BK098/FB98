namespace FB98.Modules.Movies.Domain.Enums
{
	public enum AgeRating
	{
		/// <summary>
		/// Phim được phép phổ biến đến người xem ở mọi độ tuổi.
		/// </summary>
		P = 0,
		/// <summary>
		/// Phim được phổ biến đến người xem dưới 13 tuổi và có người bảo hộ đi kèm.
		/// </summary>
		K = 1,
		/// <summary>
		/// Phim được phổ biến đến người xem từ đủ 13 tuổi trở lên (13+).
		/// </summary>
		T13 = 2,
		/// <summary>
		/// Phim được phổ biến đến người xem từ đủ 16 tuổi trở lên (16+).
		/// </summary>
		T16 = 3,
		/// <summary>
		/// Phim được phổ biến đến người xem từ đủ 18 tuổi trở lên (18+).
		/// </summary>
		T18 = 4,
		/// <summary>
		/// Phim không được phép phổ biến.
		/// </summary>
		C = 5
	}
}