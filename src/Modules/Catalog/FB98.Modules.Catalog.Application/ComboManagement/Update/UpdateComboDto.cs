namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	public class UpdateComboDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public string? ImageUrl { get; set; }
		public bool IsEnabled { get; set; }
		public ICollection<UpdateComboProductDto>? Products { get; set; }
	}

	public class UpdateComboProductDto
	{
		public Guid? ProductId { get; set; }
		public int? Quantity { get; set; }
	}
}