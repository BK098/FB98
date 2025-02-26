using AutoMapper;
using FB98.Modules.Movies.Application.CastManagement.Create;
using FB98.Modules.Movies.Application.CastManagement.GetDetail;
using FB98.Modules.Movies.Application.CastManagement.Update;
using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.CastManagement
{
	internal class CastProfile : Profile
	{
		public CastProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<Cast, GetDetailCastResponse>();
			CreateMap<CreateCastDto, Cast>();
			CreateMap<UpdateCastDto, Cast>();
		}
	}
}