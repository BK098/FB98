using AutoMapper;
using FB98.Modules.Tickets.Application.BookingManagement.SeatReservation;
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
			CreateMap<SeatReservationDto, Booking>()
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));
		}
	}
}