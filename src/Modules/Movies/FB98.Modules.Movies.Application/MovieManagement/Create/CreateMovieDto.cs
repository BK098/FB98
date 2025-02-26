using FB98.Modules.Movies.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace FB98.Modules.Movies.Application.MovieManagement.Create
{
	public class CreateMovieDto
	{
		public string Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime ReleaseDate { get; set; }
		public int RuntimeMinutes { get; set; }
		public bool IsPublished { get; set; }
		public string TrailerLink { get; set; }
		public bool? IsVietSub { get; set; } //
		public IFormFile? HeaderImage { get; set; } //
		public IFormFile? PosterImage { get; set; } //
		public string? Description { get; set; } //
		public string? Language { get; set; } //

		public string MovieGenresJson { get; set; }
		public string MovieDirectorsJson { get; set; }
		public string MovieCastsJson { get; set; }

		private List<CreateMovieCastDto> _casts = new();
		private List<CreateMovieGenreDto> _genres = new();
		private List<CreateMovieDirectorDto> _directors = new();

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<CreateMovieGenreDto> Genres
		{
			get => _genres;
			set => _genres = value;
		}

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<CreateMovieCastDto> Casts
		{
			get => _casts;
			set => _casts = value;
		}

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<CreateMovieDirectorDto> Directors
		{
			get => _directors;
			set => _directors = value;
		}

		public class CreateMovieGenreDto
		{
			public Guid Id { get; set; }
		}

		public class CreateMovieCastDto
		{
			public Guid? Id { get; set; }
			public string? Name { get; set; }
		}

		public class CreateMovieDirectorDto
		{
			public Guid? Id { get; set; }
			public string? Name { get; set; }
		}
		public void Deserialize()
		{
			if (!string.IsNullOrEmpty(MovieCastsJson))
			{
				try
				{
					_casts = JsonSerializer.Deserialize<List<CreateMovieCastDto>>(MovieCastsJson) ?? new List<CreateMovieCastDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					_casts = new List<CreateMovieCastDto>();
				}
			}
			if (!string.IsNullOrEmpty(MovieDirectorsJson))
			{
				try
				{
					_directors = JsonSerializer.Deserialize<List<CreateMovieDirectorDto>>(MovieDirectorsJson) ?? new List<CreateMovieDirectorDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					_directors = new List<CreateMovieDirectorDto>();
				}
			}
			if (!string.IsNullOrEmpty(MovieGenresJson))
			{
				try
				{
					_genres = JsonSerializer.Deserialize<List<CreateMovieGenreDto>>(MovieGenresJson) ?? new List<CreateMovieGenreDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					_genres = new List<CreateMovieGenreDto>();
				}
			}
		}
	}
}