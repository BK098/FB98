using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.Domain.Entities;

namespace FB98.Modules.Cinemas.Application.CinemaManagement.Create
{
	public sealed class CreateCinemaCommandHandler : ICommandHandler<CreateCinemaCommand, ApiResult<object>>
	{
		private readonly ICinemaRepository _cinemaRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCinemaCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateCinemaDto> _validator;

		public CreateCinemaCommandHandler(
			IValidator<CreateCinemaDto> validator,
			ILogger<CreateCinemaCommandHandler> logger,
			ICinemaRepository cinemaRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper,
			IUnitOfWork unitOfWork)
		{
			_validator = validator;
			_logger = logger;
			_cinemaRepository = cinemaRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(CreateCinemaCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				if (await _cinemaRepository.IsCinemaExisted(model.Name))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				var cinema = _mapper.Map<Cinema>(model);
				await _cinemaRepository.CreateAsync(cinema);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(cinema.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating cinema");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}