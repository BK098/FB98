using FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.DeleteDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.GetAllDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.GetDetailDiscountRule;
using FB98.Modules.Catalog.Application.DiscountManagement.UpdateDiscountRule;
using FB98.Modules.Catalog.Application.ProductManagement.Create;
using FB98.Modules.Catalog.Application.ProductManagement.Delete;
using FB98.Modules.Catalog.Application.ProductManagement.GetAll;
using FB98.Modules.Catalog.Application.ProductManagement.GetAllWCategory;
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

		[HttpGet("categories")]
		public async Task<IActionResult> GetProductsWCategory([FromQuery] Filter filter)
		{
			var request = new GetAllWCategoryQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
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

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto model)
		{
			var request = new CreateProductCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{productId:guid}")]
		public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] UpdateProductDto model)
		{
			var request = new UpdateProductCommand(productId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpDelete("{productId:guid}")]
		public async Task<IActionResult> DeleteProduct(Guid productId)
		{
			var request = new DeleteProductCommand(productId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("{productId:guid}/discount-rule")]
		public async Task<IActionResult> CreateProductDiscountRule(Guid productId, [FromBody] CreateDiscountRuleDto model)
		{
			var request = new CreateDiscountRuleCommand(productId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpGet("discount-rules")]
		public async Task<IActionResult> GetAllDiscountRule([FromQuery] Filter filter)
		{
			var request = new GetAllDiscountRuleQuery(filter, false);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpGet("{productId:guid}/discount-rule")]
		public async Task<IActionResult> GetDetailProductDiscountRule(Guid productId)
		{
			var request = new GetDetailDiscountRuleQuery(productId, false);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("discount-rule/{ruleId:guid}")]
		public async Task<IActionResult> UpdateProductDiscountRule(Guid ruleId, [FromBody] UpdateDiscountRuleDto model)
		{
			var request = new UpdateDiscountRuleCommand(ruleId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("discount-rule/{ruleId:guid}")]
		public async Task<IActionResult> DeleteProductDiscountRule(Guid ruleId)
		{
			var request = new DeleteDiscountRuleCommand(ruleId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}