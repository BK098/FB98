using Microsoft.AspNetCore.Http;

namespace FB98.Modules.Catalog.Application.ProductManagement.Create
{
	public class CreateProductDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public Guid? CategoryId { get; set; }
		public bool? IsEnabled { get; set; } = true;
		public int? Quantity { get; set; } = 0;
		public IFormFile? ProductImage { get; set; }
	}
}
