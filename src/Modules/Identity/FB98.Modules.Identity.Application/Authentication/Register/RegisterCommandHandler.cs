using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Authentication.Register
{
	internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, ApiResult<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<RegisterCommandHandler> _logger;
		private readonly IValidator<RegisterDto> _validator;
		private readonly ILocalizedMessageService _localizedMessageService;
		public RegisterCommandHandler(UserManager<AppUser> userManager,
			ILogger<RegisterCommandHandler> logger,
			IValidator<RegisterDto> validator,
			ILocalizedMessageService localizedMessageService)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResult<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}

				var existingUser = await _userManager.FindByEmailAsync(model.Email!);
				if (existingUser != null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("EmailAlreadyExists"), statusCode: 409);
				}

				var user = new AppUser
				{
					UserName = model.Email,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					Firstname = model.Firstname!,
					Lastname = model.Lastname!,
					Age = CaculatorAge(model.BirthOfDate),
					BirthOfDate = model.BirthOfDate
				};
				var result = await _userManager.CreateAsync(user, model.Password!);

				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserCreationFailed"), statusCode: 400);
				}
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("AccountCreatedSuccessfully"), statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Register");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
		private static byte CaculatorAge(DateOnly birthOfDate)
		{
			var currentDate = DateOnly.FromDateTime(DateTime.Today);
			int age = currentDate.Year - birthOfDate.Year;

			if (currentDate < birthOfDate.AddYears(age))
			{
				--age;
			}
			return (byte)age;
		}
	}
}
