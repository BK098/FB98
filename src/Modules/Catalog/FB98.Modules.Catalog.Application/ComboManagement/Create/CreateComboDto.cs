namespace FB98.Modules.Catalog.Application.ComboManagement.Create
{
	public class CreateComboDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public string? ImageUrl { get; set; }
		public bool IsEnabled { get; set; }
		public ICollection<CreateComboProductDto>? Products { get; set; }
	}

	public class CreateComboProductDto
	{
		public Guid? ProductId { get; set; }
		public int? Quantity { get; set; }
	}
}