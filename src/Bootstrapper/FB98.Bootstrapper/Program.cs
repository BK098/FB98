using FB98.Modules.Customers.Api;
using FB98.Modules.Identity.Api;
using FB98.Shared.Infrastructure;
using FB98.Shared.Infrastructure.Configurations;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();
builder.Services.AddCustomCors(builder.Configuration);


builder.Services.AddInfrastructure();
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCustomersModule(builder.Configuration);

var app = builder.Build();


//default
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.UseRouting();
app.UseCustomCors();
app.UseCustomSwagger();
//Module
app.UseIdentityModule();
app.UseCustomersModule();
app.UseInfrastructure();

//default
app.Run();
