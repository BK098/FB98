using FB98.Modules.Identity.Application.Authentication.ForgotPassword;
using FB98.Modules.Identity.Application.Authentication.RefreshToken;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Web;

namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ApiResponse<object>>
	{
		private readonly IValidator<ResetPasswordDto> _validator;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<ResetPasswordCommandHandler> _logger;
		public ResetPasswordCommandHandler(
			IValidator<ResetPasswordDto> validator,
			ILocalizedMessageService localizedMessage,
			UserManager<AppUser> userManager,
			ILogger<ResetPasswordCommandHandler> logger)
		{
			_validator = validator;
			_localizedMessage = localizedMessage;
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
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}

				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}

				var decodedToken = HttpUtility.UrlDecode(model.Token);
				var result = await _userManager.ResetPasswordAsync(user, decodedToken!, model.Password!);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("PasswordResetFailed"),
						errors: result.Errors.ToDictionary(e => e.Code, e => new List<object> 
						{ 
							e.Description 
						}), statusCode: 400
					);
				}
				return ApiResponseBuilder.Success<object>("", "Password reset successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An Error occurred: Reset Password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
