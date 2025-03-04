using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.CategoryManagement.Update
{
	internal sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, ApiResult<object>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateCategoryCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateCategoryDto> _validator;

		public UpdateCategoryCommandHandler(
			ILogger<UpdateCategoryCommandHandler> logger,
			ICategoryRepository categoryRepository,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			IValidator<UpdateCategoryDto> validator,
			ILocalizedMessageService localizedMessage)
		{
			_logger = logger;
			_categoryRepository = categoryRepository;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_validator = validator;
			_localizedMessageService = localizedMessage;
		}

		public async Task<ApiResult<object>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var categoryId = request.CategoryId;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var category = await _categoryRepository.GetByIdAsync(categoryId);
				if (category is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (category.Name != model.Name)
				{
					var categoryExisted = await _categoryRepository.IsCategoryExistsAsync(model.Name!, cancellationToken);
					if (!categoryExisted)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
					}
				}

				_mapper.Map(model, category);
				_categoryRepository.Update(category);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(categoryId, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update category");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}