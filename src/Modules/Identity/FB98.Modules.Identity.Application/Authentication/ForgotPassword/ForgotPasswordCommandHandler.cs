using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Email;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Web;

namespace FB98.Modules.Identity.Application.Authentication.ForgotPassword
{
	internal class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<ForgotPasswordCommandHandler> _logger;
		private readonly IValidator<ForgotPasswordDto> _validator;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly IConfiguration _configuration;
		private readonly IEmailSender _emailSender;
		public ForgotPasswordCommandHandler(UserManager<AppUser> userManager,
			ILogger<ForgotPasswordCommandHandler> logger,
			IValidator<ForgotPasswordDto> validator,
			ILocalizedMessageService localizedMessage,
			IConfiguration configuration,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessage = localizedMessage;
			_configuration = configuration;
			_emailSender = emailSender;
		}
		public async Task<ApiResponse<object>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
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

				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var encodedToken = HttpUtility.UrlEncode(token);
				var resetLink = $"{_configuration["FrontendBaseUrl"]}/reset-password?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(model.Email)}";

				_logger.LogInformation(HttpUtility.HtmlDecode(encodedToken));
				await _emailSender.SendEmailAsync(user.Email!, "Reset Password", resetLink);

				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("PasswordResetLinkSent"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while forgot password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
