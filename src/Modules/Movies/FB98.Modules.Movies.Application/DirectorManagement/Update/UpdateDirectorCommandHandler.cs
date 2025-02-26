using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;

namespace FB98.Modules.Movies.Application.DirectorManagement.Update
{
	internal sealed class UpdateDirectorCommandHandler : ICommandHandler<UpdateDirectorCommand, ApiResult<object>>
	{
		private readonly IDirectorRepository _directorRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateDirectorCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateDirectorDto> _validator;

		public UpdateDirectorCommandHandler(IDirectorRepository directorRepository, ILocalizedMessageService localizedMessageService, ILogger<UpdateDirectorCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork, IValidator<UpdateDirectorDto> validator)
		{
			_directorRepository = directorRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UpdateDirectorCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var directorId = request.DirectorId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var director = await _directorRepository.GetByIdAsync(directorId);
				if (director == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (director.Name != model.Name && await _directorRepository.IsDirectorExistsAsync(model.Name!))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				if (director.Name == model.Name)
				{
					return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
				}

				_mapper.Map(model, director);
				_directorRepository.Update(director);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update cast");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}