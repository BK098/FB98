using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Authentication.RevokeToken
{
	internal class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		public RevokeTokenCommandHandler(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}
		public async Task<ApiResponse<object>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(request.UserId);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>("User not found", statusCode: 404);
				}

				user.RefreshToken = null;
				user.IsRevoked = true;
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success<object>("", "Token revoked successfully");
			}
			catch
			{
				throw;
			}
		}
	}
}
