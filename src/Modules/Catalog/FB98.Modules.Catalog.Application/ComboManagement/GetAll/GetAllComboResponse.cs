using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Catalog.Application.ComboManagement.GetAll
{
	public class GetAllComboResponse : IResponse
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public string? Image { get; set; }
	}
}