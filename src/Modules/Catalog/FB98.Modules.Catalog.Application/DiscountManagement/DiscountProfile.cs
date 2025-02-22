using FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.DiscountManagement
{
	public class DiscountProfile : Profile
	{
		public DiscountProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateDiscountRuleDto, ProductDiscountRule>();
		}
	}
}