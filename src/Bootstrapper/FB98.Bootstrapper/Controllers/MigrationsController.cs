using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Modules.Warehouse.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Bootstrapper.Controllers
{
	[ApiController]
	[Route("api/migrations")]
	public class MigrationsController : ControllerBase
	{
		private readonly CatalogModuleDbContext _catalogContext;
		private readonly OrdersModuleDbContext _ordersContext;
		private readonly WarehouseModuleDbContext _warehouseContext;
		public MigrationsController(CatalogModuleDbContext catalogContext, OrdersModuleDbContext ordersContext, WarehouseModuleDbContext warehouseContext)
		{
			_catalogContext = catalogContext;
			_ordersContext = ordersContext;
			_warehouseContext = warehouseContext;
		}

		[HttpPost("seed-data")]
		public async Task<IActionResult> SeedData()
		{
			try
			{
				await CatalogSeeder.SeedDataAsync(_catalogContext);
				await OrdersSeeder.SeedDataAsync(_ordersContext);
				await WarehouseSeeder.SeedDataAsync(_warehouseContext);
				return Ok(new { message = "Seed data inserted successfully!" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = ex.Message });
			}
		}

	}
}
