namespace FB98.Modules.Catalog.Application.ComboManagement.GetDetail
{
	public record GetDetailComboQuery(Guid ComboId) : IQuery<ApiResponse<GetDetailComboResponse>>;
}
