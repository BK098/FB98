using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	public class UpdateComboDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public IFormFile? ComboImage { get; set; }
		public bool IsEnabled { get; set; }
		public string ProductsJson { get; set; } = string.Empty;

		private List<UpdateComboProductDto> _products = new();

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<UpdateComboProductDto> Products
		{
			get => _products;
			set => _products = value;
		}
		public void DeserializeProducts()
		{
			if (!string.IsNullOrEmpty(ProductsJson))
			{
				try
				{
					_products = JsonSerializer.Deserialize<List<UpdateComboProductDto>>(ProductsJson) ?? new List<UpdateComboProductDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"JSON Parsing Error: {ex.Message}");
					_products = new List<UpdateComboProductDto>();
				}
			}
		}
	}
	public class UpdateComboProductDto
	{
		public Guid? ProductId { get; set; }
		public int? Quantity { get; set; }
	}
}