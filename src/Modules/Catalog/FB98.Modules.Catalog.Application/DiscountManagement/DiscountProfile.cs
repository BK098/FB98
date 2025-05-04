using FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.GetDetailDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.UpdateDiscountRule;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.DiscountManagement
{
	internal sealed class DiscountProfile : Profile
	{
		public DiscountProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateDiscountRuleDto, ProductDiscountRule>();
			CreateMap<UpdateDiscountRuleDto, ProductDiscountRule>();
			CreateMap<ProductDiscountRule, GetDetailDiscountRuleResponse>();
		}
	}
}