using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Cloudinaries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.Application.MovieManagement.Create
{
	internal sealed class CreateMovieCommandHandler : ICommandHandler<CreateMovieCommand, ApiResult<object>>
	{
		private readonly ICastRepository _castRepository;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly IDirectorRepository _directorRepository;
		private readonly IGenreRepository _genreRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateMovieCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMovieRepository _movieRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateMovieDto> _validator;

		public CreateMovieCommandHandler(
			IMapper mapper,
			ILogger<CreateMovieCommandHandler> logger,
			IMovieRepository movieRepository,
			ICastRepository castRepository,
			IDirectorRepository directorRepository,
			IGenreRepository genreRepository,
			IUnitOfWork unitOfWork,
			IValidator<CreateMovieDto> validator,
			ICloudinaryService cloudinaryService,
			ILocalizedMessageService localizedMessageService)
		{
			_mapper = mapper;
			_logger = logger;
			_movieRepository = movieRepository;
			_castRepository = castRepository;
			_directorRepository = directorRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_cloudinaryService = cloudinaryService;
			_localizedMessageService = localizedMessageService;
			_genreRepository = genreRepository;
		}

		public async Task<ApiResult<object>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			string? headerImageUrl = null;
			string? postermageUrl = null;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var genreIds = model.Genres.Select(g => g.Id).ToList();
				var existingGenres = await _genreRepository.GetAll()
					.Where(g => genreIds.Contains(g.Id)).ToListAsync(cancellationToken);
				if (existingGenres.Count != genreIds.Count)
				{
					return ApiResponseBuilder.Error<object>("genreIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var castIds = model.Casts.Select(c => c.Id).ToList();
				var existingCasts = await _castRepository.GetAll()
					.Where(g => castIds.Contains(g.Id)).ToListAsync(cancellationToken);
				if (existingCasts.Count != castIds.Count)
				{
					return ApiResponseBuilder.Error<object>("castIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var directorIds = model.Directors.Select(d => d.Id).ToList();
				var existingDirectors = await _directorRepository.GetAll()
					.Where(d => directorIds.Contains(d.Id)).ToListAsync(cancellationToken);
				if (existingDirectors.Count != directorIds.Count)
				{
					return ApiResponseBuilder.Error<object>("directorIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var movie = _mapper.Map<Movie>(model);

				await _movieRepository.CreateAsync(movie);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(movie.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating movie");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}