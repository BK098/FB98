using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Identity.Application.ProfileManagement.ChangePassword
{
	public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<ChangePasswordCommandHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly IValidator<ChangePasswordDto> _validator;
		public ChangePasswordCommandHandler(
			UserManager<AppUser> userManager,
			ILogger<ChangePasswordCommandHandler> logger,
			ILocalizedMessageService localizedMessage,
			IValidator<ChangePasswordDto> validator)
		{
			_userManager = userManager;
			_logger = logger;
			_localizedMessage = localizedMessage;
			_validator = validator;
		}
		public async Task<ApiResponse<object>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}
				var user = await _userManager.FindByIdAsync(request.UserId);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}

				var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword!);
				if (!passwordCheck)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("InvalidPassword"), statusCode: 400);
				}

				var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("PasswordChangeFailed"),
						errors: result.Errors.ToDictionary(e =>
							e.Code, e => new List<object>
							{
								e.Description
							}), statusCode: 400);
				}

				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("PasswordChanged"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: change password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
