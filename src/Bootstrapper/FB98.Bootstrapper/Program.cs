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
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		In = ParameterLocation.Header,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Description = "Nhập Bearer Token zô đây đi",
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
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
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
