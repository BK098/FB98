using FB98.Modules.Catalog.Application.ProductManagement.Create;
using FB98.Modules.Catalog.Application.ProductManagement.Delete;
using FB98.Modules.Catalog.Application.ProductManagement.GetAll;
using FB98.Modules.Catalog.Application.ProductManagement.GetDetail;
using FB98.Modules.Catalog.Application.ProductManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Catalog.Api.Controllers
{
	internal class ProductsController : BaseController
	{
		public ProductsController(IMediator mediator) : base(mediator)
		{
		}
		[HttpGet("{productId}")]
		public async Task<IActionResult> GetProduct(Guid productId)
		{
			var request = new GetDetailProductQuery(productId);
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
		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto model)
		{
			var request = new CreateProductCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
		[HttpPut("{productId}")]
		public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] UpdateProductDto model)
		{
			var request = new UpdateProductCommand(productId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
		[HttpDelete("{productId}")]
		public async Task<IActionResult> DeleteProduct(Guid productId)
		{
			var request = new DeleteProductCommand(productId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}
