using FB98.Modules.Identity.Api;
using FB98.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddInfrastructure();
builder.Services.AddIdentityModule();
builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5000); // HTTP
	options.ListenAnyIP(5001, listenOptions =>
	{
		listenOptions.UseHttps("/app/Certificates/aspnetapp.pfx", null);
	});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseIdentityModule();
app.UseInfrastructure();

app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
app.MapGet("/", () => "Hello, HTTPS World!");
app.Run();
