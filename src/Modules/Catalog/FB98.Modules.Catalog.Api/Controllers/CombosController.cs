using FB98.Modules.Catalog.Application.ComboManagement.Create;
using FB98.Modules.Catalog.Application.ComboManagement.Delete;
using FB98.Modules.Catalog.Application.ComboManagement.GetAll;
using FB98.Modules.Catalog.Application.ComboManagement.GetDetail;
using FB98.Modules.Catalog.Application.ComboManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Catalog.Api.Controllers
{
	internal class CombosController : BaseController
	{
		public CombosController(IMediator mediator) : base(mediator) { }

		[HttpPost]
		public async Task<IActionResult> CreateCombo([FromForm] CreateComboDto model)
		{
			model.DeserializeProducts();
			var request = new CreateComboCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
		[HttpGet("{comboId}")]
		public async Task<IActionResult> GetCombo(Guid comboId)
		{
			var request = new GetDetailComboQuery(comboId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
		[HttpGet]
		public async Task<IActionResult> GetCombos([FromQuery] Filter filter)
		{
			var request = new GetAllComboQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
		[HttpPut("{comboId}")]
		public async Task<IActionResult> UpdateCombo(Guid comboId, [FromForm] UpdateComboDto model)
		{
			model.DeserializeProducts();
			var request = new UpdateComboCommand(comboId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
		[HttpDelete("{comboId}")]
		public async Task<IActionResult> DeleteCombo(Guid comboId)
		{
			var request = new DeleteComboCommand(comboId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}
