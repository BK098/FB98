using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.CategoryManagement.Delete
{
	internal sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, ApiResult<object>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteCategoryCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteCategoryCommandHandler(
			ILogger<DeleteCategoryCommandHandler> logger,
			ICategoryRepository categoryRepository,
			IUnitOfWork unitOfWork,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_categoryRepository = categoryRepository;
			_unitOfWork = unitOfWork;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<object>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			var categoryId = request.CategoryId;
			try
			{
				var category = await _categoryRepository.GetByIdAsync(categoryId);
				if (category is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_categoryRepository.Delete(category);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting category");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting category");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}