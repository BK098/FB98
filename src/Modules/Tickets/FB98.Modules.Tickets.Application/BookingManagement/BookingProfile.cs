using AutoMapper;
using FB98.Modules.Tickets.Application.BookingManagement.Create;
using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.BookingManagement
{
	internal sealed class BookingProfile : Profile
	{
		public BookingProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateBookingDto, Booking>()
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));
		}
	}
}
