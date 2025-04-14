using AutoMapper;
using FB98.Modules.Payments.Application.CouponManagement.Create;
using FB98.Modules.Payments.Application.CouponManagement.GetAll;
using FB98.Modules.Payments.Application.CouponManagement.GetDetail;
using FB98.Modules.Payments.Application.CouponManagement.Update;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Utils.Extensions;

namespace FB98.Modules.Payments.Application.CouponManagement
{
	internal sealed class CouponProfile : Profile
	{
		public CouponProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<Coupon, GetAllCouponResponse>();
			CreateMap<UpdateCouponDto, Coupon>();
			CreateMap<CreateCouponDto, Coupon>();
			CreateMap<Coupon, GetDetailCouponResponse>()
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")))
				.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")));
		}
	}
}