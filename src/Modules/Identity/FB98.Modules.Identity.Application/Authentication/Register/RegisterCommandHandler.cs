using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Identity.Application.Authentication.Register
{
	public class RegisterCommandHandler : ICommandHandler<RegisterCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<RegisterCommandHandler> _logger;
		private readonly IValidator<RegisterDto> _validator;
		private readonly ILocalizedMessageService _localizedMessage;
		public RegisterCommandHandler(UserManager<AppUser> userManager,
			ILogger<RegisterCommandHandler> logger,
			IValidator<RegisterDto> validator,
			ILocalizedMessageService localizedMessage)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessage = localizedMessage;
		}
		public async Task<ApiResponse<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}

				var existingUser = await _userManager.FindByEmailAsync(model.Email!);
				if (existingUser != null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("EmailAlreadyExists"), statusCode: 400);
				}

				var user = new AppUser
				{
					UserName = model.Email,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					Firstname = model.Firstname!,
					Lastname = model.Lastname!,
					Age = (byte)model.Age,
					RefreshToken = default!
				};
				var result = await _userManager.CreateAsync(user, model.Password!);

				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("UserCreationFailed"),
						errors: result.Errors.ToDictionary(
							e => e.Code,
							e => new List<object> { e.Description }
						),
						statusCode: 400);
				}
				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("AccountCreatedSuccessfully"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during registration");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
