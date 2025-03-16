using FB98.Modules.Tickets.Application.SeatPriceRules.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Tickets.Api.Controllers
{
	internal class SeatPriceRulesController : BaseController
	{
		public SeatPriceRulesController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateSeatPriceRule([FromBody] CreateRuleDto model)
		{
			var request = new CreateRuleCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}