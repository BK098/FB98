using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetAll
{
	public class GetAllProductResponse : IResponse
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Image { get; set; }
		public decimal Price { get; set; }
		public decimal DiscountPrice { get; set; }
	}
}