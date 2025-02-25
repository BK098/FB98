using AutoMapper;
using FB98.Modules.Cinemas.Application.CinemaManagement.Create;
using FB98.Modules.Cinemas.Application.CinemaManagement.GetAll;
using FB98.Modules.Cinemas.Application.CinemaManagement.GetDetail;
using FB98.Modules.Cinemas.Application.CinemaManagement.Update;
using FB98.Modules.Cinemas.Domain.Entities;

namespace FB98.Modules.Cinemas.Application.CinemaManagement
{
	internal sealed class CinemaProfile : Profile
	{
		public CinemaProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateCinemaDto, Cinema>();
			CreateMap<UpdateCinemaDto, Cinema>();
			CreateMap<Cinema, GetDetailCinemaResponse>()
				.ForMember(dest => dest.Halls, opt => opt.MapFrom(src => src.CinemaHalls));
			CreateMap<CinemaHall, HallDto>()
				.ForMember(dest => dest.HallId, opt => opt.MapFrom(src => src.Id));
			CreateMap<Cinema, GetAllCinemaResponse>();
		}
	}
}