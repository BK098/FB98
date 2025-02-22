using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace FB98.Modules.Catalog.Application.ComboManagement.Create
{
	public class CreateComboDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public IFormFile? ComboImage { get; set; }
		public bool IsEnabled { get; set; }
		public string ProductsJson { get; set; } = string.Empty;

		private List<CreateComboProductDto> _products = new();

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<CreateComboProductDto> Products
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
					_products = JsonSerializer.Deserialize<List<CreateComboProductDto>>(ProductsJson) ?? new List<CreateComboProductDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					_products = new List<CreateComboProductDto>();
				}
			}
		}
	}

	public class CreateComboProductDto
	{
		public Guid? ProductId { get; set; }
		public int? Quantity { get; set; }
	}
}