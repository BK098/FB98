namespace FB98.Modules.Catalog.Application.ProductManagement.GetDetail
{
	public class GetDetailProductResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? Image { get; set; }

		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; } = default!;

		public int RemainingQuantity { get; set; }
	}
}
