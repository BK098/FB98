using FB98.Modules.Warehouse.Application.InventoryManagement.AddStock;
using FB98.Modules.Warehouse.Application.InventoryManagement.GetStock;
using FB98.Modules.Warehouse.Application.InventoryManagement.ReduceStock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Warehouse.Api.Controllers
{
	internal class InventoriesController : BaseController
	{
		public InventoriesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet("get-stock/{productId:guid}")]
		public async Task<IActionResult> ReduceStock(Guid productId)
		{
			var request = new GetStockQuery(productId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpPost("add-stock")]
		public async Task<IActionResult> AddStock(AddStockDto model)
		{
			var request = new AddStockCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "adminstrator")]
		[HttpPost("reduce-stock")]
		public async Task<IActionResult> ReduceStock(ReduceStockDto model)
		{
			var request = new ReduceStockCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}