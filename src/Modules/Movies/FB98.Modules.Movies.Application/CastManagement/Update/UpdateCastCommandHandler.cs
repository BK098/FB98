using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Application.CastManagement.Create;

namespace FB98.Modules.Movies.Application.CastManagement.Update
{
	internal sealed class UpdateCastCommandHandler : ICommandHandler<UpdateCastCommand, ApiResult<object>>
	{
		private readonly ICastRepository _castRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCastCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateCastDto> _validator;

		public UpdateCastCommandHandler(ICastRepository castRepository, ILocalizedMessageService localizedMessageService, ILogger<CreateCastCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork, IValidator<UpdateCastDto> validator)
		{
			_castRepository = castRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UpdateCastCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var castId = request.CastId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var cast = await _castRepository.GetByIdAsync(castId);
				if (cast == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (cast.Name != model.Name && await _castRepository.IsCastExistsAsync(model.Name!))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				if (cast.Name == model.Name)
				{
					return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
				}

				_mapper.Map(model, cast);
				_castRepository.Update(cast);
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