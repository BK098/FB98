using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.ProfileManagement.GetProfile
{
	internal class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ApiResult<GetProfileResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetProfileQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly UserManager<AppUser> _userManager;

		public GetProfileQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetProfileQueryHandler> logger,
			IMapper mapper,
			UserManager<AppUser> userManager)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_userManager = userManager;
		}

		public async Task<ApiResult<GetProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
		{
			var searchTerm = request.Model.SearchTerm?.Trim();
			try
			{
				AppUser? user = null;

				if (!string.IsNullOrEmpty(searchTerm))
				{
					if (Guid.TryParse(searchTerm, out _))
					{
						user = await _userManager.FindByIdAsync(searchTerm);
					}

					if (user == null)
					{
						user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == searchTerm);
					}

					if (user == null)
					{
						user = await _userManager.FindByEmailAsync(searchTerm);
					}
				}

				if (user == null)
				{
					return ApiResponseBuilder.Error<GetProfileResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetProfileResponse>(user);
				return ApiResponseBuilder.Success(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get user");
				return ApiResponseBuilder.Error<GetProfileResponse>("An unexpected error occurred", 500);
			}
		}
	}
}