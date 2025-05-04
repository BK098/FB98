using FB98.Bootstrapper.Extensions;
using FB98.Modules.Catalog.Api;
using FB98.Modules.Cinemas.Api;
using FB98.Modules.Customers.Api;
using FB98.Modules.Identity.Api;
using FB98.Modules.Movies.Api;
using FB98.Modules.Orders.Api;
using FB98.Modules.Payments.Api;
using FB98.Modules.Shows.Api;
using FB98.Modules.Systems.Api;
using FB98.Modules.Tickets.Api;
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
builder.Services.AddCinemaModule(builder.Configuration);
builder.Services.AddMovieModule(builder.Configuration);
builder.Services.AddShowModule(builder.Configuration);
builder.Services.AddTicketModule(builder.Configuration);
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
app.UseCinemaModule();
app.UseMovieModule();
app.UseShowModule();
app.UseTicketModule();
app.UseSystemModule();
//default
app.UseEndpoints(endpoints =>
{
	endpoints?.MapControllers();
	endpoints?.MapSignalRHubs();
});
app.Run();