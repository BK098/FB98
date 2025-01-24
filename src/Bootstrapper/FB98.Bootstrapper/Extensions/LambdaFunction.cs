using Amazon.Lambda.AspNetCoreServer;

namespace FB98.Bootstrapper.Extensions
{
	public class LambdaFunction: APIGatewayProxyFunction
	{
		protected override void Init(IWebHostBuilder builder)
		{
			builder
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Program>()
				.UseLambdaServer();
		}
	}
}
