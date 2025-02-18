using Refit;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("FB98.Modules.Orders.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface ICatalogApi
	{
		[Get("/catalog-module/Products/{productId}")]
		Task<Responses.ApiResult<ProductDto>> GetProductById(Guid productId);
		[Get("/catalog-module/Combos/{comboId}")]
		Task<Responses.ApiResult<ComboDto>> GetComboById(Guid comboId);
	}
	public class ProductDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
	}
	public class ComboDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
		public List<ComboProductDto> Products { get; set; } = new List<ComboProductDto>();
	}
	public class ComboProductDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
