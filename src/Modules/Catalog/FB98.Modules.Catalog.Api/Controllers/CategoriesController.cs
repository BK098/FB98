using FB98.Modules.Catalog.Application.CategoryManagement.Create;
using FB98.Modules.Catalog.Application.CategoryManagement.Delete;
using FB98.Modules.Catalog.Application.CategoryManagement.GetAll;
using FB98.Modules.Catalog.Application.CategoryManagement.GetDetail;
using FB98.Modules.Catalog.Application.CategoryManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Catalog.Api.Controllers
{
	internal class CategoriesController : BaseController
	{
		public CategoriesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetCategoriess([FromQuery] Filter filter)
		{
			var request = new GetAllCategoryQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{categoryId}")]
		public async Task<IActionResult> GetCategory(Guid categoryId)
		{
			var request = new GetDetailCategoryQuery(categoryId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpPost]
		public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto model)
		{
			var request = new CreateCategoryCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpPut]
		public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryDto model)
		{
			var request = new UpdateCategoryCommand(categoryId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpDelete]
		public async Task<IActionResult> DeleteCategory(Guid categoryId)
		{
			var request = new DeleteCategoryCommand(categoryId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}