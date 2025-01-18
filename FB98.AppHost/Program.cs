using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FB98_Bootstrapper>("fb98-bootstrapper");

//check heath cuar Postgre
builder.Build().Run();
