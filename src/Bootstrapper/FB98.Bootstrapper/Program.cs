using FB98.Bootstrapper.Extensions;
using FB98.Modules.Catalog.Api;
using FB98.Modules.Customers.Api;
using FB98.Modules.Identity.Api;
using FB98.Modules.Orders.Api;
using FB98.Modules.Payments.Api;
using FB98.Modules.ShoppingList.Api;
using FB98.Modules.Systems.Api;
using FB98.Modules.Warehouse.Api;
using FB98.Shared.Infrastructure;
using FB98.Shared.Infrastructure.Configurations;
using FB98.Shared.Infrastructure.SignalRHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomSwagger();
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCustomersModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddWarehouseModule(builder.Configuration);
builder.Services.AddOrderModule(builder.Configuration);
builder.Services.AddPaymentModule(builder.Configuration);
builder.Services.AddShoppingListModule(builder.Configuration);
builder.Services.AddSystemModule(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddRegisterServices();
var app = builder.Build();

//default
app.UseInfrastructure();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCustomCors();
app.UseRouting();
app.UseCustomSwagger();

//Module
app.UseIdentityModule();
app.UseCustomersModule();
app.UseCatalogModule();
app.UseWarehouseModule();
app.UseOrderModule();
app.UsePaymentModule();
app.UseShoppingListModule();
app.UseSystemModule();
//default
app.UseEndpoints(endpoints =>
{
	endpoints?.MapControllers();
	endpoints?.MapSignalRHubs();
});
app.Run();