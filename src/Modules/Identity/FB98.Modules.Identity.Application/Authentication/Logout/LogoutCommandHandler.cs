using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Domain.Entities;
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
		private readonly ITokenStoreRepository _tokenStoreRepository;
		public LogoutCommandHandler(UserManager<AppUser> userManager,
			ILocalizedMessageService localizedMessage,
			ITokenStoreRepository tokenStoreRepository)
		{
			_userManager = userManager;
			_localizedMessage = localizedMessage;
			_tokenStoreRepository = tokenStoreRepository;
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


				//await _tokenStoreRepository.RevokeByDeviceIdAsync();
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
