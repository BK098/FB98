using FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule;
using FB98.Modules.Catalog.Application.ProductManagement.Create;
using FB98.Modules.Catalog.Application.ProductManagement.Delete;
using FB98.Modules.Catalog.Application.ProductManagement.GetAll;
using FB98.Modules.Catalog.Application.ProductManagement.GetDetail;
using FB98.Modules.Catalog.Application.ProductManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Catalog.Api.Controllers
{
	internal class ProductsController : BaseController
	{
		public ProductsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetProducts([FromQuery] Filter filter)
		{
			var request = new GetAllProductQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{productId:guid}")]
		public async Task<IActionResult> GetProduct(Guid productId)
		{
			var request = new GetDetailProductQuery(productId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}


		[Authorize(Roles = "adminstrator")]
		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto model)
		{
			var request = new CreateProductCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpPost("{productId:guid}/discount-rule")]
		public async Task<IActionResult> CreateProductDiscountRule(Guid productId, [FromBody] CreateDiscountRuleDto model)
		{
			var request = new CreateDiscountRuleCommand(productId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpPut("{productId:guid}")]
		public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] UpdateProductDto model)
		{
			var request = new UpdateProductCommand(productId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpDelete("{productId}")]
		public async Task<IActionResult> DeleteProduct(Guid productId)
		{
			var request = new DeleteProductCommand(productId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}