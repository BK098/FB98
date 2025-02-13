using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Authentication.Logout
{
	internal sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILocalizedMessageService _localizedMessageService;
		public LogoutCommandHandler(UserManager<AppUser> userManager,
			ILocalizedMessageService localizedMessageService)
		{
			_userManager = userManager;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResponse<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(request.UserId);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}


				//await _tokenStoreRepository.RevokeByDeviceIdAsync();
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("LoggedOut"), statusCode: 200);
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
