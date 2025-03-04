using FB98.Modules.Shows.Application.FeatureTypeManagement.Create;
using FB98.Modules.Shows.Application.FeatureTypeManagement.Delete;
using FB98.Modules.Shows.Application.FeatureTypeManagement.GetAll;
using FB98.Modules.Shows.Application.FeatureTypeManagement.GetDetail;
using FB98.Modules.Shows.Application.FeatureTypeManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Shows.Api.Controllers
{
	internal class FeatureTypesController : BaseController
	{
		public FeatureTypesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetFeatureTypes([FromQuery] Filter filter)
		{
			var request = new GetAllFeatureTypeQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{featureTypeId:guid}")]
		public async Task<IActionResult> GetFeatureType(Guid featureTypeId)
		{
			var request = new GetDetailFeatureTypeQuery(featureTypeId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateFeatureType([FromBody] CreateFeatureTypeDto model)
		{
			var request = new CreateFeatureTypeCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{featureTypeId:guid}")]
		public async Task<IActionResult> UpdateFeatureType(Guid featureTypeId, [FromBody] UpdateFeatureTypeDto model)
		{
			var request = new UpdateFeatureTypeCommand(featureTypeId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("{featureTypeId:guid}")]
		public async Task<IActionResult> DeleteFeatureType(Guid featureTypeId)
		{
			var request = new DeleteFeatureTypeCommand(featureTypeId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}