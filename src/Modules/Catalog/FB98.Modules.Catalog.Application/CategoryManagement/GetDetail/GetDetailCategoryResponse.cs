using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Catalog.Application.CategoryManagement.GetDetail
{
	public class GetDetailCategoryResponse : IResponse
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public int ProductCount { get; set; }
	}
}