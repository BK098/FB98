
using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Infrastructure.Cloudinaries;

namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	internal sealed class UpdateComboCommandHandler : ICommandHandler<UpdateComboCommand, ApiResponse<object>>
	{
		private readonly IMapper _mapper;
		private readonly ILogger<UpdateComboCommandHandler> _logger;
		private readonly IComboRepository _comboRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateComboDto> _validator;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;
		public UpdateComboCommandHandler(
			IMapper mapper,
			ILogger<UpdateComboCommandHandler> logger,
			IComboRepository comboRepository,
			IProductRepository productRepository,
			IUnitOfWork unitOfWork,
			IValidator<UpdateComboDto> validator,
			ICloudinaryService cloudinaryService,
			ILocalizedMessageService localizedMessageService)
		{
			_mapper = mapper;
			_logger = logger;
			_comboRepository = comboRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_cloudinaryService = cloudinaryService;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResponse<object>> Handle(UpdateComboCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var comboId = request.ComboId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}
				var combo = await _comboRepository.GetByIdAsync(comboId);
				if (combo is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}
				_mapper.Map(model, combo);
				string? imageUrl;
				if (model.ComboImage != null)
				{
					if (combo.Image != null)
					{
						imageUrl = await _cloudinaryService.ReplaceImageAsync(model.ComboImage!, "catalog/combo", combo.Image);
						combo.Image = imageUrl;
					}
					else
					{
						imageUrl = await _cloudinaryService.UploadImageAsync(model.ComboImage!, "catalog/combo");
						combo.Image = imageUrl;
					}
				}
				_comboRepository.Update(combo);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"), statusCode: 200);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all products");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
