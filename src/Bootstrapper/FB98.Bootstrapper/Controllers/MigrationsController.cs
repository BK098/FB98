using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Customers.DataAccess.Data;
using FB98.Modules.Identity.DataAccess.Data;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Modules.Payments.DataAccess.Data;
using FB98.Modules.Warehouse.DataAccess.Data;
using FB98.Shared.Abstractions.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Bootstrapper.Controllers
{
	[ApiController]
	[Route("system-module/migrations")]
	public class MigrationsController : ControllerBase
	{
		private readonly CatalogModuleDbContext _catalogContext;
		private readonly CustomerModuleDbContext _customerContext;
		private readonly IdentityModuleDbContext _identityContext;
		private readonly OrderModuleDbContext _orderContext;
		private readonly PaymentModuleDbContext _paymentContext;
		private readonly WarehouseModuleDbContext _warehouseContext;

		public MigrationsController(CatalogModuleDbContext catalogContext, OrderModuleDbContext orderContext, WarehouseModuleDbContext warehouseContext, PaymentModuleDbContext paymentContext, IdentityModuleDbContext identityContext, CustomerModuleDbContext customerContext)
		{
			_catalogContext = catalogContext;
			_orderContext = orderContext;
			_warehouseContext = warehouseContext;
			_paymentContext = paymentContext;
			_identityContext = identityContext;
			_customerContext = customerContext;
		}

		[HttpPost("seed-data")]
		public async Task<IActionResult> SeedData()
		{
			try
			{
				await CatalogSeeder.SeedDataAsync(_catalogContext);
				await OrdersSeeder.SeedDataAsync(_orderContext);
				await WarehouseSeeder.SeedDataAsync(_warehouseContext);
				await PaymentSeeder.SeedDataAsync(_paymentContext);
				await IdentitySeeder.SeedDataAsync(_identityContext);
				await CustomerSeeder.SeedDataAsync(_customerContext);
				var response = new ApiResult<object>
				{
					Message = "Seed data inserted successfully!",
					StatusCode = 200,
					IsSuccess = true
				};
				return StatusCode(response.StatusCode, response);
			}
			catch (Exception ex)
			{
				var response = new ApiResult<object>
				{
					Message = $"{ex}",
					StatusCode = 500,
					IsSuccess = false
				};
				return StatusCode(response.StatusCode, response);
			}
		}
	}
}