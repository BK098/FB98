using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Cinemas.DataAccess.Data;
using FB98.Modules.Identity.DataAccess.Data;
using FB98.Modules.Movies.DataAccess.Data;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Modules.Payments.DataAccess.Data;
using FB98.Modules.Shows.DataAccess.Data;
using FB98.Modules.Warehouse.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Bootstrapper.Controllers
{
	[ApiController]
	[Route("api/migrations")]
	public class MigrationsController : ControllerBase
	{
		private readonly CatalogModuleDbContext _catalogContext;
		private readonly CinemaModuleDbContext _cinemaContext;
		private readonly IdentityModuleDbContext _identityContext;
		private readonly MovieModuleDbContext _movieContext;
		private readonly OrdersModuleDbContext _ordersContext;
		private readonly PaymentModuleDbContext _paymentContext;
		private readonly ShowModuleDbContext _showContext;
		private readonly WarehouseModuleDbContext _warehouseContext;

		public MigrationsController(
			CatalogModuleDbContext catalogContext,
			OrdersModuleDbContext ordersContext,
			WarehouseModuleDbContext warehouseContext,
			PaymentModuleDbContext paymentContext,
			CinemaModuleDbContext cinemaContext,
			IdentityModuleDbContext identityContext,
			MovieModuleDbContext movieContext,
			ShowModuleDbContext showContext)
		{
			_catalogContext = catalogContext;
			_ordersContext = ordersContext;
			_warehouseContext = warehouseContext;
			_paymentContext = paymentContext;
			_cinemaContext = cinemaContext;
			_identityContext = identityContext;
			_movieContext = movieContext;
			_showContext = showContext;
		}

		[HttpPost("seed-data")]
		public async Task<IActionResult> SeedData()
		{
			try
			{
				await CatalogSeeder.SeedDataAsync(_catalogContext);
				await OrdersSeeder.SeedDataAsync(_ordersContext);
				await WarehouseSeeder.SeedDataAsync(_warehouseContext);
				await PaymentSeeder.SeedDataAsync(_paymentContext);
				await CinemaSeeder.SeedDataAsync(_cinemaContext);
				await IdentitySeeder.SeedDataAsync(_identityContext);
				await MovieSeeder.SeedDataAsync(_movieContext);
				await ShowSeeder.SeedDataAsync(_showContext);

				return Ok(new { message = "Seed data inserted successfully!" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = ex.Message });
			}
		}
	}
}