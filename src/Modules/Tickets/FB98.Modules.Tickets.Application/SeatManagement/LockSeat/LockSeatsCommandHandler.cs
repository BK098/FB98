using FB98.Modules.Tickets.Application.Abstractions;

namespace FB98.Modules.Tickets.Application.SeatManagement.LockSeat
{
	internal sealed class LockSeatsCommandHandler : ICommandHandler<LockSeatsCommand, ApiResult<object>>
	{
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<LockSeatsCommandHandler> _logger;
		//private readonly IValidator<LockSeatsDto> _validator;

		public LockSeatsCommandHandler(
			IBookingSeatLockRepository bookingSeatLockRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<LockSeatsCommandHandler> logger)
		//IValidator<LockSeatsDto> validator)
		{
			_bookingSeatLockRepository = bookingSeatLockRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			//_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(LockSeatsCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				//var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				//if (!validationResult.IsValid)
				//{
				//	return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				//}

				var success = await _bookingSeatLockRepository.LockSeats(model.CustomerId, model.ShowId, model.SeatIds);
				if (!success)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SeatsAlreadyLocked"));
				}

				return ApiResponseBuilder.Success<object>("", "");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while locking seats");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}