using AutoMapper;
using FB98.Modules.Shows.Application.FeatureManagement.Create;
using FB98.Modules.Shows.Application.FeatureManagement.GetAll;
using FB98.Modules.Shows.Application.FeatureManagement.GetDetail;
using FB98.Modules.Shows.Application.FeatureManagement.Update;
using FB98.Modules.Shows.Domain.Entities;

namespace FB98.Modules.Shows.Application.FeatureManagement
{
	internal sealed class FeatureProfile : Profile
	{
		public FeatureProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateFeatureDto, Feature>();
			CreateMap<UpdateFeatureDto, Feature>();
			CreateMap<Feature, GetDetailFeatureResponse>()
				.ForMember(dest => dest.FeatureTypeId, opt => opt.MapFrom(src => src.FeatureTypeId))
				.ForMember(dest => dest.FeatureTypeName, opt => opt.MapFrom(src => src.FeatureType.Name));
			CreateMap<Feature, GetAllFeatureResponse>();
		}
	}
}