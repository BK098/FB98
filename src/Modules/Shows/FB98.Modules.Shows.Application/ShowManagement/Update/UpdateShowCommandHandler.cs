using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace FB98.Modules.Shows.Application.ShowManagement.Update
{
	internal sealed class UpdateShowCommandHandler : ICommandHandler<UpdateShowCommand, ApiResult<object>>
	{
		private readonly ICinemaApi _cinemaApi;
		private readonly IFeatureRepository _featureRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateShowCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMovieApi _movieApi;
		private readonly IShowRepository _showRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateShowDto> _validator;

		public UpdateShowCommandHandler(
			ICinemaApi cinemaApi,
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<UpdateShowCommandHandler> logger,
			IMapper mapper,
			IMovieApi movieApi,
			IShowRepository showRepository,
			IUnitOfWork unitOfWork,
			IValidator<UpdateShowDto> validator)
		{
			_cinemaApi = cinemaApi;
			_featureRepository = featureRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_movieApi = movieApi;
			_showRepository = showRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UpdateShowCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var showId = request.ShowId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var show = await _showRepository.GetByIdAsync(showId);
				if (show == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var overlappingShows = await _showRepository.GetAll()
					.Where(s => s.CinemaHallId == model.CinemaHallId &&
								s.Id != showId &&
								((s.StartTime >= model.StartTime && s.StartTime < model.EndTime) ||
								 (s.EndTime > model.StartTime && s.EndTime <= model.EndTime)))
					.ToListAsync(cancellationToken);

				if (overlappingShows.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("ShowOverlap"), 400);
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

				_mapper.Map(model, show);
				var runtime = movieResponse.Data!.RuntimeMinutes;

				show.CinemaHallName = hallResponse.Data!.Name;
				show.MovieTitle = movieResponse.Data!.Title;
				show.MovieRuntimeMinutes = runtime;
				show.EndTime = show.StartTime.AddMinutes(runtime);
				show.ShowStatusId = ShowStatusConstants.UpComming;

				await UpdateFeatures(show, model.Features!);

				_unitOfWork.Entry(show, EntityState.Modified);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(show.Id, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update show");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}

		private async Task UpdateFeatures(Show show, ICollection<UpdateShowFeatureDto> features)
		{
			var existingFeatureIds = show.Features.Select(c => c.FeatureId).ToList();
			var newFeatureIds = features.Select(c => c.FeatureId!.Value).ToList();

			var featureToRemove = show.Features.Where(c => !newFeatureIds.Contains(c.FeatureId)).ToList();
			foreach (var feature in featureToRemove)
			{
				_unitOfWork.Entry(feature, EntityState.Deleted);
			}

			var featureMembersToAdd = newFeatureIds.Except(existingFeatureIds).ToList();
			var allFeatureMembers = await _featureRepository.GetByIdsAsync(featureMembersToAdd);

			foreach (var featureId in featureMembersToAdd)
			{
				var featureMember = allFeatureMembers.FirstOrDefault(cm => cm.Id == featureId);
				if (featureMember != null)
				{
					var newMovieFeatureMember = new ShowFeature
					{
						FeatureId = featureMember.Id,
						ShowId = show.Id
					};
					_unitOfWork.Entry(newMovieFeatureMember, EntityState.Added);
				}
			}
		}
	}
}