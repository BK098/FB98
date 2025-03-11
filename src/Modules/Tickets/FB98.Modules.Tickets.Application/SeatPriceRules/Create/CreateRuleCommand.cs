namespace FB98.Modules.Tickets.Application.SeatPriceRules.Create
{
	public record CreateRuleCommand(CreateRuleDto Model) : ICommand<ApiResult<object>>;
}