using AutoMapper;
using FB98.Modules.Customers.Application.CustomerManagement.GetDetail;
using FB98.Modules.Customers.Domain.Entities;

namespace FB98.Modules.Customers.Application.CustomerManagement
{
	internal sealed class CustomerProfile : Profile
	{
		public CustomerProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<Customer, GetDetailCustomerResponse>()
				.ForMember(dest => dest.MembershipDiscount, opt => opt.MapFrom(src => src.Membership.DiscountRate))
				.ForMember(dest => dest.Membership, opt => opt.MapFrom(src => src.Membership.LevelName));
		}
	}
}