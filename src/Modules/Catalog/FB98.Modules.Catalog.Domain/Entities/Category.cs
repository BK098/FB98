using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Category : BaseEntity
	{
		[StringLength(255)]
		public string Name { get; set; } = null!;

		public ICollection<Product> Products { get; set; } = new List<Product>();

		public int GetTotalProducts()
		{
			return Products.Count;
		}
	}
}