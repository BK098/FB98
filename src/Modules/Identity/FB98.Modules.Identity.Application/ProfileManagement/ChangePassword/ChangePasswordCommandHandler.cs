using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.ProfileManagement.ChangePassword
{
	internal sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<ChangePasswordCommandHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IValidator<ChangePasswordDto> _validator;
		public ChangePasswordCommandHandler(
			UserManager<AppUser> userManager,
			ILogger<ChangePasswordCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IValidator<ChangePasswordDto> validator)
		{
			_userManager = userManager;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
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
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}
				var user = await _userManager.FindByIdAsync(request.UserId);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}

				var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword!);
				if (!passwordCheck)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("InvalidPassword"), statusCode: 400);
				}

				var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("PasswordChangeFailed"), statusCode: 400);
				}

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("PasswordChanged"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: change password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
