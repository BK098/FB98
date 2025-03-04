using AutoMapper;
using FB98.Modules.Movies.Application.DirectorManagement.Create;
using FB98.Modules.Movies.Application.DirectorManagement.GetAll;
using FB98.Modules.Movies.Application.DirectorManagement.GetDetail;
using FB98.Modules.Movies.Application.DirectorManagement.Update;
using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.DirectorManagement
{
	internal sealed class DirectorProfile : Profile
	{
		public DirectorProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<Director, GetDetailDirectorResponse>();
			CreateMap<Director, GetAllDirectorResponse>();
			CreateMap<CreateDirectorDto, Director>();
			CreateMap<UpdateDirectorDto, Director>();
		}
	}
}