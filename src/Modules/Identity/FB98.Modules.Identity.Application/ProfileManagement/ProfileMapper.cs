using FB98.Modules.Identity.Application.ProfileManagement.GetProfile;
using FB98.Modules.Identity.Domain.Entities;

namespace FB98.Modules.Identity.Application.ProfileManagement
{
	internal sealed class ProfileMapper : Profile
	{
		public ProfileMapper()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<AppUser, GetProfileResponse>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id.ToString()))
				.ForMember(dest => dest.BirthOfDate, opt => opt.MapFrom(src => src.BirthOfDate.ToString("dd-MM-yyyy")));
		}
	}
}