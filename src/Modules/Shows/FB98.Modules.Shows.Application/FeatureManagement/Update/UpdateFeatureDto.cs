namespace FB98.Modules.Shows.Application.FeatureManagement.Update
{
	public class UpdateFeatureDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public Guid? FeatureTypeId { get; set; }
	}
}