using AutoMapper;
using FB98.Modules.Shows.Application.ShowManagement.Create;
using FB98.Modules.Shows.Application.ShowManagement.CreateRange;
using FB98.Modules.Shows.Application.ShowManagement.GetDetail;
using FB98.Modules.Shows.Application.ShowManagement.Update;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Utils.Extensions;

namespace FB98.Modules.Shows.Application.ShowManagement
{
	internal sealed class ShowProfile : Profile
	{
		public ShowProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateShowDto, Show>()
				.ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));
			CreateMap<CreateShowFeatureDto, ShowFeature>()
				.ForMember(dest => dest.ShowId, opt => opt.Ignore())
				.ForMember(dest => dest.Show, opt => opt.Ignore());

			CreateMap<UpdateShowDto, Show>()
				.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime!.Value.ToUniversalTime()))
				.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime!.Value.ToUniversalTime()))
				.ForMember(dest => dest.Features, opt => opt.Ignore());

			CreateMap<UpdateShowFeatureDto, ShowFeature>()
				.ForMember(dest => dest.Feature, opt => opt.MapFrom(src => src.FeatureId))
				.ForMember(dest => dest.ShowId, opt => opt.Ignore())
				.ForMember(dest => dest.Show, opt => opt.Ignore());

			CreateMap<CreateRangeShowDto, Show>()
				.ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));

			CreateMap<CreateRangeShowFeatureDto, ShowFeature>()
				.ForMember(dest => dest.ShowId, opt => opt.Ignore())
				.ForMember(dest => dest.Show, opt => opt.Ignore());

			CreateMap<Show, GetDetailShowResponse>()
				.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")))
				.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")))
				.ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));

			CreateMap<ShowFeature, GetDetailShowFeatureResponse>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FeatureId))
				.ForMember(dest => dest.FeatureName, opt => opt.MapFrom(src => src.Feature!.Name));

			//CreateMap<Show, GetAllShowResponse>();

			//CreateMap<Show, GetAllShowDto>()
			//	.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")))
			//	.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")))
			//	.ForMember(dest => dest.ShowId, opt => opt.MapFrom(src => src.Id))
			//	.ForMember(dest => dest.ShowStatusId, opt => opt.MapFrom(src => src.ShowStatusId))
			//	.ForMember(dest => dest.ShowStatusName, opt => opt.MapFrom(src => src.ShowStatus!.Name));
		}
	}
}