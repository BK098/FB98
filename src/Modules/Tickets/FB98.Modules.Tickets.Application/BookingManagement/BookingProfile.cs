using AutoMapper;
using FB98.Modules.Tickets.Application.BookingManagement.GetDetail;
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
			CreateMap<Booking, GetDetailBookingResponse>()
				.ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.BookingSeats));
			CreateMap<BookingSeat, GetDetailBookingSeatResponse>();
		}
	}
}