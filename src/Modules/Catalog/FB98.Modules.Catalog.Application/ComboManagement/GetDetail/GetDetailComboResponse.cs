namespace FB98.Modules.Catalog.Application.ComboManagement.GetDetail
{
	public class GetDetailComboResponse
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public string? Image { get; set; }

		public ICollection<GetDetailComboProductResponse> Products { get; set; } = new List<GetDetailComboProductResponse>();
	}
	public class GetDetailComboProductResponse
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Image { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
