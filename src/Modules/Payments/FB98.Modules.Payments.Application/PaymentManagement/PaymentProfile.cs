using AutoMapper;
using FB98.Modules.Payments.Application.PaymentManagement.GetDetail;
using FB98.Modules.Payments.Application.PaymentManagement.GetPaymentHisotry;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Utils.Extensions;

namespace FB98.Modules.Payments.Application.PaymentManagement
{
	internal sealed class PaymentProfile : Profile
	{
		public PaymentProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<PaymentTransaction, GetPaymentHisotryResponse>()
				.ForMember(dest => dest.PairedAt, opt => opt.MapFrom(src => src.CreateAt!.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")))
				.ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.PaymentMethod!.Name))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PaymentStatus!.Name));

			CreateMap<BookingDetailDto, GetDeteailBookingPaymentResponse>()
				.ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));

			CreateMap<BookingSeatDetailDto, GetDeteailBookingSeatPaymentResponse>()
				.ForMember(dest => dest.SeatPosition, opt => opt.MapFrom(src => src.SeatPosition))
				.ForMember(dest => dest.SeatTypeName, opt => opt.MapFrom(src => src.SeatTypeName))
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

			CreateMap<OrderDetailDto, GetDeteailOrderPaymentResponse>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

			CreateMap<OrderDetailItemDto, GetDeteailOrderItemPaymentResponse>()
				.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
				.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
				.ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
				.ForMember(dest => dest.IsCombo, opt => opt.MapFrom(src => src.IsCombo));
		}
	}
}
