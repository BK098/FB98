using AutoMapper;
using FB98.Modules.Movies.Application.MovieManagement.Create;
using FB98.Modules.Movies.Application.MovieManagement.GetAll;
using FB98.Modules.Movies.Application.MovieManagement.GetDetail;
using FB98.Modules.Movies.Application.MovieManagement.Update;
using FB98.Modules.Movies.Domain.Entities;

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
			CreateMap<CreateMovieDto.CreateMovieGenreDto, MovieGenre>()
				.ForMember(src => src.Id, opt => opt.Ignore())
				.ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
			CreateMap<CreateMovieDto.CreateMovieCastDto, MovieCast>()
				.ForMember(src => src.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CastId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
			CreateMap<CreateMovieDto.CreateMovieDirectorDto, MovieDirector>()
				.ForMember(src => src.Id, opt => opt.Ignore())
				.ForMember(dest => dest.DirectorId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());

			CreateMap<UpdateMovieDto, Movie>()
				.ForMember(dest => dest.Casts, opt => opt.Ignore())
				.ForMember(dest => dest.Genres, opt => opt.Ignore())
				.ForMember(dest => dest.Directors, opt => opt.Ignore());
			CreateMap<UpdateMovieGenreDto, MovieGenre>()
				.ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Id, opt => opt.Ignore())
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
			CreateMap<UpdateMovieCastDto, MovieCast>()
				.ForMember(dest => dest.CastId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Id, opt => opt.Ignore())
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());
			CreateMap<UpdateMovieDirectorDto, MovieDirector>()
				.ForMember(dest => dest.DirectorId, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Id, opt => opt.Ignore())
				.ForMember(src => src.Movie, opt => opt.Ignore())
				.ForMember(dest => dest.MovieId, opt => opt.Ignore());

			CreateMap<Movie, GetDetailMovieResponse>()
				.ForMember(dest => dest.Casts, opt => opt.MapFrom(src => src.Casts))
				.ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
				.ForMember(dest => dest.Directors, opt => opt.MapFrom(src => src.Directors));
			CreateMap<MovieCast, GetDetailMovieCastResponse>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CastId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Cast.Name));
			CreateMap<MovieDirector, GetDetailMovieDirectorResponse>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DirectorId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Director.Name));
			CreateMap<MovieGenre, GetDetailMovieGenreResponse>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.GenreId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Genre.Name));

			CreateMap<Movie, GetAllMovieResponse>()
				.ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres));
			CreateMap<MovieGenre, GetAllMovieGenreResponse>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.GenreId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Genre.Name));
		}
	}
}