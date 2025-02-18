using AutoMapper;
using FB98.Modules.Orders.Application.OrderManagement.Create;
using FB98.Modules.Orders.Domain.Entities;

namespace FB98.Modules.Orders.Application.OrderManagement
{
	public sealed class OrderProfile : Profile
	{
		public OrderProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateOrderDto, Order>()
				.ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));
			CreateMap<CreateOrderItemDto, OrderItem>()
				.ForMember(src => src.Order, opt => opt.Ignore())
				.ForMember(dest => dest.OrderId, opt => opt.Ignore());
		}
	}
}
