using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.CategoryManagement.Create
{
	internal sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, ApiResult<object>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateCategoryCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateCategoryDto> _validator;

		public CreateCategoryCommandHandler(
			ILogger<CreateCategoryCommandHandler> logger,
			IUnitOfWork unitOfWork,
			IValidator<CreateCategoryDto> validator,
			ICategoryRepository categoryRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_categoryRepository = categoryRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<object>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var categoryExisted = await _categoryRepository.IsCategoryExistsAsync(model.Name!, cancellationToken);
				if (categoryExisted)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				var category = _mapper.Map<Category>(model);
				await _categoryRepository.CreateAsync(category);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(category.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create category");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}