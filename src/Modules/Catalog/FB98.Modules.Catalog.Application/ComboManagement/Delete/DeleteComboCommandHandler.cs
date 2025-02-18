using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Infrastructure.Cloudinaries;

namespace FB98.Modules.Catalog.Application.ComboManagement.Delete
{
	internal sealed class DeleteComboCommandHandler : ICommandHandler<DeleteComboCommand, ApiResult<object>>
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<DeleteComboCommandHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IComboRepository _comboRepository;
		private readonly ICloudinaryService _cloudinaryService;
		public DeleteComboCommandHandler(
			IUnitOfWork unitOfWork,
			ILogger<DeleteComboCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IComboRepository comboRepository,
			ICloudinaryService cloudinaryService)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_comboRepository = comboRepository;
			_cloudinaryService = cloudinaryService;
		}

		public async Task<ApiResult<object>> Handle(DeleteComboCommand request, CancellationToken cancellationToken)
		{
			var comboId = request.ComboId;
			try
			{
				var combo = await _comboRepository.GetByIdAsync(comboId);
				if (combo is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

				_cloudinaryService.DeleteImage(combo.Image);
				_comboRepository.Delete(combo);

				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"), statusCode: 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delele combo");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
