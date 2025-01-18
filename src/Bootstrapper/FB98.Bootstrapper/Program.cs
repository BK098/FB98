using FB98.Modules.Identity.Api;
using FB98.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddInfrastructure();
builder.Services.AddIdentityModule();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseIdentityModule();
app.UseInfrastructure();

app.MapControllers();
app.Run();
