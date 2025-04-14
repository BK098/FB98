using AutoMapper;
using FB98.Modules.Orders.Application.OrderManagement.Create;
using FB98.Modules.Orders.Application.OrderManagement.GetAllOrder;
using FB98.Modules.Orders.Application.OrderManagement.GetDetail;
using FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Utils.Extensions;

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
			CreateMap<OrderStatusHistory, GetOrderStatusHistoryResponse>()
				.ForMember(dest => dest.ChangedAt, opt => opt.MapFrom(src => src.CreateAt))
				.ForMember(dest => dest.OldStatus, opt => opt.MapFrom(src => OrderStatusConstants.GetStatusName(src.OldStatusId)))
				.ForMember(dest => dest.NewStatus, opt => opt.MapFrom(src => OrderStatusConstants.GetStatusName(src.NewStatusId)));
			CreateMap<CreateOrderDto, Order>()
				.ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));
			CreateMap<CreateOrderItemDto, OrderItem>()
				.ForMember(src => src.Order, opt => opt.Ignore())
				.ForMember(dest => dest.OrderId, opt => opt.Ignore());
			CreateMap<Order, GetDetailOrderResponse>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
				.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.OrderStatusId));

			CreateMap<OrderItem, GetDetailOrderItemResponse>();

			CreateMap<Order, GetAllOrderResponse>()
				.ForMember(dest => dest.OrderStatusName, opt => opt.MapFrom(src => src.OrderStatus!.Name))
				.ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss")));
		}
	}
}