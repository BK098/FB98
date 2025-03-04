using AutoMapper;
using FB98.Modules.Shows.Application.FeatureTypeManagement.Create;
using FB98.Modules.Shows.Application.FeatureTypeManagement.GetAll;
using FB98.Modules.Shows.Application.FeatureTypeManagement.GetDetail;
using FB98.Modules.Shows.Application.FeatureTypeManagement.Update;
using FB98.Modules.Shows.Domain.Entities;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement
{
	internal sealed class FeatureTypeProfile : Profile
	{
		public FeatureTypeProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateFeatureTypeDto, FeatureType>();
			CreateMap<UpdateFeatureTypeDto, FeatureType>();
			CreateMap<FeatureType, GetDetailFeatureTypeResponse>();
			CreateMap<FeatureType, GetAllFeatureTypeResponse>();
		}
	}
}