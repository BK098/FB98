using AutoMapper;
using FB98.Modules.Movies.Application.MovieManagement.Create;
using FB98.Modules.Movies.Domain.Entities;
using static FB98.Modules.Movies.Application.MovieManagement.Create.CreateMovieDto;

namespace FB98.Modules.Movies.Application.MovieManagement
{
	internal class MovieProfile : Profile
	{
		public MovieProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateMovieDto, Movie>()
				.ForMember(dest => dest.Casts, opt => opt.MapFrom(src => src.Casts))
				.ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
				.ForMember(dest => dest.Directors, opt => opt.MapFrom(src => src.Directors));
			CreateMap<CreateMovieGenreDto, MovieGenre>()
				.ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
			CreateMap<CreateMovieCastDto, MovieCast>()
				.ForMember(dest => dest.CastId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
			CreateMap<CreateMovieDirectorDto, MovieDirector>()
				.ForMember(dest => dest.DirectorId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
		}
	}
}