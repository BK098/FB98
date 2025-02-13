namespace FB98.Modules.Catalog.Application.ProductManagement.GetAll
{
	public class GetAllProductResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string? Image { get; set; }
		public decimal Price { get; set; }
	}
}
