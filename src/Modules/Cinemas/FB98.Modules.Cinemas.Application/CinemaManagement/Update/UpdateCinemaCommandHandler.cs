using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;

namespace FB98.Modules.Cinemas.Application.CinemaManagement.Update
{
	internal sealed class UpdateCinemaCommandHandler : ICommandHandler<UpdateCinemaCommand, ApiResult<object>>
	{
		private readonly ICinemaRepository _cinemaRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateCinemaCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateCinemaDto> _validator;

		public UpdateCinemaCommandHandler(
			ICinemaRepository cinemaRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<UpdateCinemaCommandHandler> logger,
			IMapper mapper,
			IValidator<UpdateCinemaDto> validator,
			IUnitOfWork unitOfWork)
		{
			_cinemaRepository = cinemaRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_validator = validator;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(UpdateCinemaCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var cinemaId = request.CinemaId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var cinema = await _cinemaRepository.GetByIdAsync(cinemaId);
				if (cinema == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}
				if (cinema.Name != model.Name && await _cinemaRepository.IsCinemaExisted(model.Name))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				_mapper.Map(model, cinema);
				_cinemaRepository.Update(cinema);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update cinema");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}