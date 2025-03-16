using FB98.Modules.Tickets.Application.Abstractions;

namespace FB98.Modules.Tickets.Application.SeatManagement.UnlockSeat
{
	internal sealed class UnlockSeatsCommandHandler : ICommandHandler<UnlockSeatsCommand, ApiResult<object>>
	{
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UnlockSeatsCommandHandler> _logger;
		private readonly IValidator<UnlockSeatsDto> _validator;

		public UnlockSeatsCommandHandler(
			IBookingSeatLockRepository bookingSeatLockRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<UnlockSeatsCommandHandler> logger,
			IValidator<UnlockSeatsDto> validator)
		{
			_bookingSeatLockRepository = bookingSeatLockRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UnlockSeatsCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var unlocked = await _bookingSeatLockRepository.UnlockSeats(model.CustomerId!.Value, model.ShowId!.Value, model.SeatIds!);
				if (!unlocked)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SeatsNotLocked"));
				}

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("SeatsUnlocked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while unlocking seats");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}
