using FB98.Modules.Warehouse.Application.InventoryManagement.AddStock;
using FB98.Modules.Warehouse.Application.InventoryManagement.CreateInventory;
using FB98.Modules.Warehouse.Application.InventoryManagement.GetStock;
using FB98.Modules.Warehouse.Application.InventoryManagement.ReduceStock;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Warehouse.Api.Controllers
{
	internal class InventoriesController : BaseController
	{
		public InventoriesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet("get-stock/{productId}")]
		public async Task<IActionResult> ReduceStock(Guid productId)
		{
			var request = new GetStockQuery(productId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("add-stock")]
		public async Task<IActionResult> AddStock(AddStockDto model)
		{
			var request = new AddStockCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateInventory(CreateInventoryDto model)
		{
			var request = new CreateInventoryCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("reduce-stock")]
		public async Task<IActionResult> ReduceStock(ReduceStockDto model)
		{
			var request = new ReduceStockCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}
