using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace FB98.Modules.Shows.Application.ShowManagement.CreateRange
{
	internal sealed class CreateRangeShowCommandHandler : ICommandHandler<CreateRangeShowCommand, ApiResult<object>>
	{
		private readonly ICinemaApi _cinemaApi;
		private readonly IFeatureRepository _featureRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateRangeShowCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMovieApi _movieApi;
		private readonly IShowRepository _showRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateRangeShowDto> _validator;

		public CreateRangeShowCommandHandler(
			IFeatureRepository featureRepository,
			ILogger<CreateRangeShowCommandHandler> logger,
			IShowRepository showRepository,
			IValidator<CreateRangeShowDto> validator,
			ILocalizedMessageService localizedMessageService,
			ICinemaApi cinemaApi,
			IMovieApi movieApi,
			IUnitOfWork unitOfWork,
			IMapper mapper)
		{
			_featureRepository = featureRepository;
			_logger = logger;
			_showRepository = showRepository;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
			_cinemaApi = cinemaApi;
			_movieApi = movieApi;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResult<object>> Handle(CreateRangeShowCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				ApiResult<CinemaHallDto>? hallResponse;
				try
				{
					hallResponse = await _cinemaApi.GetHallById(model.CinemaHallId!.Value);
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("Hall: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				ApiResult<MovieDto>? movieResponse;
				try
				{
					movieResponse = await _movieApi.GetMovieById(model.MovieId!.Value);
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("Movie: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var featureIds = model.Features.Select(g => g.FeatureId).ToList();
				var existingGenres = await _featureRepository.GetAll()
					.Where(g => featureIds.Contains(g.Id)).ToListAsync(cancellationToken);
				if (existingGenres.Count != featureIds.Count)
				{
					return ApiResponseBuilder.Error<object>("Feature: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var shows = new List<Show>();

				var currentStartTime = model.StartDate;

				while (currentStartTime < model.EndDate)
				{
					var runtime = movieResponse.Data!.RuntimeMinutes;
					var showName = currentStartTime.ToString("HH:mm");
					var showDescription = $"{showName} - {currentStartTime.AddMinutes(runtime):HH:mm}";

					var show = new Show
					{
						Description = showDescription,
						Name = showName,
						CinemaHallId = model.CinemaHallId!.Value,
						MovieId = model.MovieId!.Value,
						CinemaHallName = hallResponse.Data!.Name,
						MovieTitle = movieResponse.Data!.Title,
						MovieRuntimeMinutes = runtime,
						StartTime = currentStartTime,
						EndTime = currentStartTime.AddMinutes(runtime),
						ShowStatusId = ShowStatusConstants.UpComming,
						Features = model.Features.Select(f => new ShowFeature
						{
							FeatureId = f.FeatureId
						}).ToList()
					};
					if (show.EndTime > model.EndDate)
					{
						break;
					}

					shows.Add(show);

					currentStartTime = show.EndTime.AddMinutes(model.TimeRest);
				}

				await _showRepository.CreateRangeAsync(shows);
				await _unitOfWork.SaveChangesAsync();

				var showIds = shows.Select(x => x.Id);
				return ApiResponseBuilder.Success<object>(showIds, _localizedMessageService.GetLocalizedMessage("Created"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating shows");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}