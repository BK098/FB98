using FB98.Modules.Identity.Application.Authentication.ForgotPassword;
using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ApiResponse<object>>
	{
		private readonly IValidator<ResetPasswordDto> _validator;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly UserManager<AppUser> _userManager;

		public ResetPasswordCommandHandler(
			IValidator<ResetPasswordDto> validator,
			ILocalizedMessageService localizedMessage,
			UserManager<AppUser> userManager)
		{
			_validator = validator;
			_localizedMessage = localizedMessage;
			_userManager = userManager;
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
					return ApiResponseBuilder.Error<object>("User not found", statusCode: 404);
				}

				var decodedToken = HttpUtility.UrlDecode(model.Token);
				var result = await _userManager.ResetPasswordAsync(user, decodedToken!, model.Password!);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>("Failed to reset password", errors: result.Errors.ToDictionary(e => e.Code, e => new List<object> { e.Description }), statusCode: 400);
				}
				return ApiResponseBuilder.Success<object>("", "Password reset successfully");
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
