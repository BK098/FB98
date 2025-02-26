using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Application.CastManagement.Create;
using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.DirectorManagement.Create
{
	internal sealed class CreateDirectorCommandHandler : ICommandHandler<CreateDirectorCommand, ApiResult<object>>
	{
		private readonly IDirectorRepository _directorRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCastCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateDirectorDto> _validator;

		public CreateDirectorCommandHandler(
			IDirectorRepository directorRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateCastCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<CreateDirectorDto> validator)
		{
			_directorRepository = directorRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(CreateDirectorCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				if (await _directorRepository.IsDirectorExistsAsync(model.Name!))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				var director = _mapper.Map<Director>(model);
				await _directorRepository.CreateAsync(director);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(director.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create director");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}