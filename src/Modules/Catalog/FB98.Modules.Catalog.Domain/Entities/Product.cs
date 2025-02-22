using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Product : BaseProduct
	{
		[ForeignKey("Category")]
		public Guid CategoryId { get; set; }
		public Category Category { get; set; } = null;

		public ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();
	}
}