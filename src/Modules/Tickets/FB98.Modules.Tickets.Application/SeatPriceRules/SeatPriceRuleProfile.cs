using AutoMapper;
using FB98.Modules.Tickets.Application.SeatPriceRules.Create;
using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.SeatPriceRules
{
	internal sealed class SeatPriceRuleProfile : Profile
	{
		public SeatPriceRuleProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateRuleDto, SeatPriceRule>();
		}
	}
}