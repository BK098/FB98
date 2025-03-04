namespace FB98.Modules.Shows.Application.FeatureManagement.GetDetail
{
	public class GetDetailFeatureResponse
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid FeatureTypeId { get; set; }
		public string FeatureTypeName { get; set; }
	}
}