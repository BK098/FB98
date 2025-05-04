using FB98.Modules.Catalog.Application.ComboManagement.Create;
using FB98.Modules.Catalog.Application.ComboManagement.Delete;
using FB98.Modules.Catalog.Application.ComboManagement.GetAll;
using FB98.Modules.Catalog.Application.ComboManagement.GetDetail;
using FB98.Modules.Catalog.Application.ComboManagement.Update;
using FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.DeleteDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.GetAllDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.GetDetailDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.UpdateDiscountRule;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Catalog.Api.Controllers
{
	internal class CombosController : BaseController
	{
		public CombosController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetCombos([FromQuery] Filter filter)
		{
			var request = new GetAllComboQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{comboId:guid}")]
		public async Task<IActionResult> GetCombo(Guid comboId)
		{
			var request = new GetDetailComboQuery(comboId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateCombo([FromBody] CreateComboDto model)
		{
			var request = new CreateComboCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{comboId:guid}")]
		public async Task<IActionResult> UpdateCombo(Guid comboId, [FromBody] UpdateComboDto model)
		{
			var request = new UpdateComboCommand(comboId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("{comboId:guid}")]
		public async Task<IActionResult> DeleteCombo(Guid comboId)
		{
			var request = new DeleteComboCommand(comboId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("{comboId:guid}/discount-rule")]
		public async Task<IActionResult> CreatecomboDiscountRule(Guid comboId, [FromBody] CreateDiscountRuleDto model)
		{
			model.SetAtCombo();
			var request = new CreateDiscountRuleCommand(comboId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("discount-rules")]
		public async Task<IActionResult> GetAllDiscountRule([FromQuery] Filter filter)
		{
			var request = new GetAllDiscountRuleQuery(filter, true);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpGet("{comboId:guid}/discount-rule")]
		public async Task<IActionResult> GetDetailComboDiscountRule(Guid comboId)
		{
			var request = new GetDetailDiscountRuleQuery(comboId, true);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("discount-rule/{ruleId:guid}")]
		public async Task<IActionResult> UpdateComboDiscountRule(Guid ruleId, [FromBody] UpdateDiscountRuleDto model)
		{
			model.SetAtCombo();
			var request = new UpdateDiscountRuleCommand(ruleId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("discount-rule/{ruleId:guid}")]
		public async Task<IActionResult> DeleteComboDiscountRule(Guid ruleId)
		{
			var request = new DeleteDiscountRuleCommand(ruleId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}