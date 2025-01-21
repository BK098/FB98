using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Authentication.Logout
{
	internal class LogoutCommandHandler : ICommandHandler<LogoutCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILocalizedMessageService _localizedMessage;
		public LogoutCommandHandler(UserManager<AppUser> userManager,
			ILocalizedMessageService localizedMessage)
		{
			_userManager = userManager;
			_localizedMessage = localizedMessage;
		}
		public async Task<ApiResponse<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(request.UserId);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}
				user.RefreshToken = null;
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("LoggedOut"));
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
