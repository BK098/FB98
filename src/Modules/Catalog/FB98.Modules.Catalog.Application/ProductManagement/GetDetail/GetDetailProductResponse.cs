using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetDetail
{
	public class GetDetailProductResponse : IResponse
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public decimal DiscountPrice { get; set; }
		public string? Image { get; set; }

		public Guid CategoryId { get; set; }
		public string? CategoryName { get; set; }
	}
}