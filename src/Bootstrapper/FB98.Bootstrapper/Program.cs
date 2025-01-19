using FB98.Modules.Identity.Api;
using FB98.Shared.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddInfrastructure();
builder.Services.AddIdentityModule(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });
	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer"
	});
	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Id = "Bearer",
					Type = ReferenceType.SecurityScheme
				}
			},
			new string[]{}
		}
	});
});
builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5000); // HTTP
	options.ListenAnyIP(5001, listenOptions =>
	{
		listenOptions.UseHttps("/app/Certificates/aspnetapp.pfx", null);
	});
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
//default
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.UseRouting();

//Module
app.UseIdentityModule();
app.UseInfrastructure();

//default
app.Run();
