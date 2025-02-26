using AutoMapper;
using FB98.Modules.Movies.Application.CastManagement.GetDetail;
using FB98.Modules.Movies.Application.DirectorManagement.Create;
using FB98.Modules.Movies.Application.DirectorManagement.Update;
using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.DirectorManagement
{
	internal class DirectorProfile : Profile
	{
		public DirectorProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<Director, GetDetailCastResponse>();
			CreateMap<CreateDirectorDto, Director>();
			CreateMap<UpdateDirectorDto, Director>();
		}
	}
}