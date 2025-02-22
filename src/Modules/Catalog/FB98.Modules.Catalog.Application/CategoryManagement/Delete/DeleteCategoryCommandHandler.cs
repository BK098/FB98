using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.CategoryManagement.Delete
{
	internal sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, ApiResult<object>>
	{
		private readonly ILogger<DeleteCategoryCommandHandler> _logger;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILocalizedMessageService _localizedMessageService;

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
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"),
						statusCode: 404);
				}

				_categoryRepository.Delete(category);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"),
					statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while forgot password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}