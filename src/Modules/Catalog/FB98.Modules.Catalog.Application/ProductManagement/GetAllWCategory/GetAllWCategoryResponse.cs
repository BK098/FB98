namespace FB98.Modules.Catalog.Application.ProductManagement.GetAllWCategory
{
	public class GetAllWCategoryResponse
	{
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; }
		public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
		public class ProductResponse
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public string? Image { get; set; }
			public decimal Price { get; set; }
			public decimal DiscountPrice { get; set; }
		}
	}
}