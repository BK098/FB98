using AutoMapper;
using FB98.Modules.Cinemas.Application.HallManagement.CheckSeats;
using FB98.Modules.Cinemas.Application.HallManagement.Create;
using FB98.Modules.Cinemas.Application.HallManagement.GetDetail;
using FB98.Modules.Cinemas.Application.HallManagement.Update;
using FB98.Modules.Cinemas.Domain.Entities;

namespace FB98.Modules.Cinemas.Application.HallManagement
{
	internal sealed class CinemaHallProfile : Profile
	{
		public CinemaHallProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CinemaHall, CheckSeatsResponse>()
				.ForMember(dest => dest.SeatIds, opt => opt.MapFrom(src => src.Seats.Select(s => new Dictionary<Guid, Guid>
				{
					{
						s.Id, s.SeatTypeId
					}
				}).ToList()));


			CreateMap<CreateHallDto, CinemaHall>();

			CreateMap<CinemaHall, GetDetailHallResponse>()
				.ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));

			CreateMap<CinemaHallSeat, GetDetailSeatDto>()
				.ForMember(dest => dest.SeatId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.SeatTypeId, opt => opt.MapFrom(src => src.SeatTypeId))
				.ForMember(dest => dest.SeatType, opt => opt.MapFrom(src => src.SeatType.Name));

			CreateMap<UpdateHallDto, CinemaHall>()
				.ForMember(dest => dest.Seats, opt => opt.Ignore());

			CreateMap<UpdateSeatDto, CinemaHallSeat>();
		}
	}
}