using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Orders.Application.OrderManagement.Create
{
	public class CreateOrderDto
	{
		[SwaggerIgnore]
		public Guid UserId { get; set; }
		public List<CreateOrderItemDto>? Items { get; set; } = new List<CreateOrderItemDto>();
	}

	public class CreateOrderItemDto
	{
		public Guid? ProductId { get; set; }
		public bool? IsCombo { get; set; }
		public int? Quantity { get; set; }
	}
}