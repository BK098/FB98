using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace FB98.Modules.Identity.Application.Authentication.ForgotPassword
{
	internal sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<ForgotPasswordCommandHandler> _logger;
		private readonly IValidator<ForgotPasswordDto> _validator;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IConfiguration _configuration;
		private readonly IEmailSender _emailSender;
		public ForgotPasswordCommandHandler(UserManager<AppUser> userManager,
			ILogger<ForgotPasswordCommandHandler> logger,
			IValidator<ForgotPasswordDto> validator,
			ILocalizedMessageService localizedMessageService,
			IConfiguration configuration,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
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
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}
				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}

				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var encodedToken = HttpUtility.UrlEncode(token);
				var resetLink = $"{_configuration["FrontendBaseUrl"]}/reset-password?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(model.Email)}";
#if DEBUG
				_logger.LogInformation("Encoded token: {EncodedToken}", HttpUtility.HtmlDecode(encodedToken));
#endif
				await _emailSender.SendEmailAsync(user.Email!, "Reset Password", resetLink);

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("PasswordResetLinkSent"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while forgot password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
