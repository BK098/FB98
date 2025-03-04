using FB98.Modules.Shows.Application.FeatureManagement.Create;
using FB98.Modules.Shows.Application.FeatureManagement.Delete;
using FB98.Modules.Shows.Application.FeatureManagement.GetAll;
using FB98.Modules.Shows.Application.FeatureManagement.GetDetail;
using FB98.Modules.Shows.Application.FeatureManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Shows.Api.Controllers
{
	internal class FeaturesController : BaseController
	{
		public FeaturesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetTypes([FromQuery] Filter filter)
		{
			var request = new GetAllFeatureQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{featureId:guid}")]
		public async Task<IActionResult> GetFeature(Guid featureId)
		{
			var request = new GetDetailFeatureQuery(featureId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateFeature([FromBody] CreateFeatureDto model)
		{
			var request = new CreateFeatureCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{featureId:guid}")]
		public async Task<IActionResult> UpdateFeature(Guid featureId, [FromBody] UpdateFeatureDto model)
		{
			var request = new UpdateFeatureCommand(featureId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("{featureId:guid}")]
		public async Task<IActionResult> DeleteFeature(Guid featureId)
		{
			var request = new DeleteFeatureCommand(featureId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}