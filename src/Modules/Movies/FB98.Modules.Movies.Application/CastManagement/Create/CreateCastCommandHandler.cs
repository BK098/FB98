using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.CastManagement.Create
{
	internal sealed class CreateCastCommandHandler : ICommandHandler<CreateCastCommand, ApiResult<object>>
	{
		private readonly ICastRepository _castRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCastCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateCastDto> _validator;

		public CreateCastCommandHandler(
			ICastRepository castRepository,
			IMapper mapper,
			ILogger<CreateCastCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IUnitOfWork unitOfWork,
			IValidator<CreateCastDto> validator)
		{
			_castRepository = castRepository;
			_mapper = mapper;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(CreateCastCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				if (await _castRepository.IsCastExistsAsync(model.Name!))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				var cast = _mapper.Map<Cast>(model);
				await _castRepository.CreateAsync(cast);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(cast.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create cast");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}