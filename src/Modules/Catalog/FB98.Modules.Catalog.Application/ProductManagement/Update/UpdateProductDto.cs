namespace FB98.Modules.Catalog.Application.ProductManagement.Update
{
	public class UpdateProductDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public Guid? CategoryId { get; set; }
		public bool? IsEnabled { get; set; }
		public int? Quantity { get; set; } = 0;
		public string? ImageUrl { get; set; }
	}
}