using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Application.ShowManagement.CreateRange;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace FB98.Modules.Shows.Application.ShowManagement.Create
{
	internal sealed class CreateShowCommandHandler : ICommandHandler<CreateShowCommand, ApiResult<object>>
	{
		private readonly ICinemaApi _cinemaApi;
		private readonly IFeatureRepository _featureRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateRangeShowCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMovieApi _movieApi;
		private readonly IShowRepository _showRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateShowDto> _validator;

		public CreateShowCommandHandler(
			ICinemaApi cinemaApi,
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateRangeShowCommandHandler> logger,
			IMovieApi movieApi,
			IShowRepository showRepository,
			IUnitOfWork unitOfWork,
			IValidator<CreateShowDto> validator,
			IMapper mapper)
		{
			_cinemaApi = cinemaApi;
			_featureRepository = featureRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_movieApi = movieApi;
			_showRepository = showRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_mapper = mapper;
		}

		public async Task<ApiResult<object>> Handle(CreateShowCommand request, CancellationToken cancellationToken)
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

				var featureIds = model.Features!.Select(g => g.FeatureId).ToList();
				var existingGenres = await _featureRepository.GetAll()
					.Where(g => featureIds.Contains(g.Id)).ToListAsync(cancellationToken);
				if (existingGenres.Count != featureIds.Count)
				{
					return ApiResponseBuilder.Error<object>("Feature: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var features = model.Features!.Select(f => new ShowFeature
				{
					FeatureId = f.FeatureId!.Value
				}).ToList();

				var currentStartTime = model.StartTime!.Value.ToUniversalTime();
				var runtime = movieResponse.Data!.RuntimeMinutes;
				var show = _mapper.Map<Show>(model);

				show.CinemaHallName = hallResponse.Data!.Name;
				show.MovieTitle = movieResponse.Data!.Title;
				show.MovieRuntimeMinutes = runtime;
				show.EndTime = show.StartTime.AddMinutes(runtime);
				show.ShowStatusId = ShowStatusConstants.UpComming;
				show.Features = features;
				show.StartTime = currentStartTime;

				var overlappingShows = await _showRepository.GetAll()
					.Where(s => s.CinemaHallId == model.CinemaHallId &&
								((s.StartTime >= currentStartTime && s.StartTime < currentStartTime.AddMinutes(runtime)) ||
								 (s.EndTime > currentStartTime && s.EndTime <= currentStartTime.AddMinutes(runtime))))
					.ToListAsync(cancellationToken);

				if (overlappingShows.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("ShowOverlap"), 400);
				}

				await _showRepository.CreateAsync(show);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(show.Id, _localizedMessageService.GetLocalizedMessage("Created"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating show");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}