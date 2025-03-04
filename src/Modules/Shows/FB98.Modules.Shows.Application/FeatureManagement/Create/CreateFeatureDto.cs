namespace FB98.Modules.Shows.Application.FeatureManagement.Create
{
	public class CreateFeatureDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public Guid? FeatureTypeId { get; set; }
	}
}