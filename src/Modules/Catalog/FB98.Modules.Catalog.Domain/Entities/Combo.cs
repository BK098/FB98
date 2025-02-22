namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Combo : BaseProduct
	{
		public ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();
	}
}