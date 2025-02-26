using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class Genre : BaseEntity
	{
		public string Name { get; set; }
		public string? Description { get; set; }
	}
	public static class GenreConstants
	{
		/// <summary>
		/// Thể loại phim hành động
		/// </summary>
		public static Guid Action = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a80");
		/// <summary>
		/// Thể loại phim hoạt họa, hoạt hình
		/// </summary>
		public static Guid Animated = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a81");
		/// <summary>
		/// Thể loại phim hài
		/// </summary>
		public static Guid Comedy = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a82");
		/// <summary>
		/// Thể loại phim chuyển thể từ chuyện tranh
		/// </summary>
		public static Guid ComicBook = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a83");
		/// <summary>
		/// Thể loại phim tài liệu
		/// </summary>
		public static Guid Documentary = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a84");
		/// <summary>
		/// Thể loại phim kịch tính
		/// </summary>
		public static Guid Drama = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a85");
		/// <summary>
		/// Thể loại phim kinh dị
		/// </summary>
		public static Guid Horror = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a86");
		/// <summary>
		/// Thể loại phim về gia đình và trẻ nhỏ
		/// </summary>
		public static Guid KidnFamily = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a87");
		/// <summary>
		/// Thể loại phim về âm nhạc
		/// </summary>
		public static Guid Musical = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a88");
		/// <summary>
		/// Thể loại phim lãng mạn
		/// </summary>
		public static Guid Romance = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a89");
		/// <summary>
		/// Thể loại phim huyền ảo
		/// </summary>
		public static Guid Fantasy = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a90");
		/// <summary>
		/// Thể loại phim khoa học viễn tưởng
		/// </summary>
		public static Guid Scifi = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a91");
		/// <summary>
		/// Thể loại phim giật gân
		/// </summary>
		public static Guid Thriller = Guid.Parse("83be4965-298f-4037-b1dc-4d9f13176a92");
	}
}