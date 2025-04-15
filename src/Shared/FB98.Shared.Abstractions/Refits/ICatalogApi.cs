using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Orders.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	public interface ICatalogApi
	{
		[Get("/catalog-module/Products/{productId}")]
		Task<ApiResult<ProductDto>> GetProductById(Guid productId);

		[Get("/catalog-module/Combos/{comboId}")]
		Task<ApiResult<ComboDto>> GetComboById(Guid comboId);
	}

	public record ProductDto(Guid Id, string Name, decimal Price, decimal DiscountPrice);
	public record ComboDto(Guid Id, string Name, decimal Price, decimal DiscountPrice, List<ComboProductDto> Products);
	public record ComboProductDto(Guid Id, string Name, decimal Price, int Quantity);
}