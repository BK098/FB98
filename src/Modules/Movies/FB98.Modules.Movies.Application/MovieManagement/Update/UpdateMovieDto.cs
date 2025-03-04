using FB98.Modules.Movies.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace FB98.Modules.Movies.Application.MovieManagement.Update
{
	public class UpdateMovieDto
	{
		public string Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime ReleaseDate { get; set; }
		public int RuntimeMinutes { get; set; }
		public bool IsPublished { get; set; }
		public string? Country { get; set; }
		public string TrailerLink { get; set; }
		public bool? IsVietSub { get; set; }
		public IFormFile? HeaderImage { get; set; }
		public IFormFile? PosterImage { get; set; }
		public string? Description { get; set; }
		public string? Language { get; set; }

		public string MovieGenresJson { get; set; }
		public string MovieDirectorsJson { get; set; }
		public string MovieCastsJson { get; set; }

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<UpdateMovieGenreDto> Genres { get; set; } = new();

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<UpdateMovieCastDto> Casts { get; set; } = new();

		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public List<UpdateMovieDirectorDto> Directors { get; set; } = new();

		public void Deserialize()
		{
			if (!string.IsNullOrEmpty(MovieCastsJson))
			{
				try
				{
					Casts = JsonSerializer.Deserialize<List<UpdateMovieCastDto>>(MovieCastsJson) ?? new List<UpdateMovieCastDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					Casts = new List<UpdateMovieCastDto>();
				}
			}

			if (!string.IsNullOrEmpty(MovieDirectorsJson))
			{
				try
				{
					Directors = JsonSerializer.Deserialize<List<UpdateMovieDirectorDto>>(MovieDirectorsJson) ?? new List<UpdateMovieDirectorDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					Directors = new List<UpdateMovieDirectorDto>();
				}
			}

			if (!string.IsNullOrEmpty(MovieGenresJson))
			{
				try
				{
					Genres = JsonSerializer.Deserialize<List<UpdateMovieGenreDto>>(MovieGenresJson) ?? new List<UpdateMovieGenreDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($@"JSON Parsing Error: {ex.Message}");
					Genres = new List<UpdateMovieGenreDto>();
				}
			}
		}
	}
	public class UpdateMovieGenreDto
	{
		public Guid? Id { get; set; }
	}

	public class UpdateMovieCastDto
	{
		public Guid? Id { get; set; }
	}

	public class UpdateMovieDirectorDto
	{
		public Guid? Id { get; set; }
	}

}