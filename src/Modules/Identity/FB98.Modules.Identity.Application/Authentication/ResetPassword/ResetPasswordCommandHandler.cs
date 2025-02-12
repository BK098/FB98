using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	internal sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ApiResponse<object>>
	{
		private readonly IValidator<ResetPasswordDto> _validator;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<ResetPasswordCommandHandler> _logger;
		public ResetPasswordCommandHandler(
			IValidator<ResetPasswordDto> validator,
			ILocalizedMessageService localizedMessageService,
			UserManager<AppUser> userManager,
			ILogger<ResetPasswordCommandHandler> logger)
		{
			_validator = validator;
			_localizedMessageService = localizedMessageService;
			_userManager = userManager;
			_logger = logger;
		}
		public async Task<ApiResponse<object>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}

				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}

				var decodedToken = HttpUtility.UrlDecode(model.Token);
				var result = await _userManager.ResetPasswordAsync(user, decodedToken!, model.Password!);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("PasswordResetFailed"), statusCode: 400
					);
				}
				return ApiResponseBuilder.Success<object>("", "Password reset successfully", statusCode: 204);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An Error occurred: Reset Password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
