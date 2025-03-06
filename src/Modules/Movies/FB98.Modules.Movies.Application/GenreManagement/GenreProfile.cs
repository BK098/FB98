using AutoMapper;
using FB98.Modules.Movies.Application.GenreManagement.GetAll;
using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.GenreManagement
{
	internal sealed class GenreProfile : Profile
	{
		public GenreProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<Genre, GetAllGenreResponse>();
		}
	}
}