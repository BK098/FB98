using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace FB98.Shared.Infrastructure.Middlewares
{
	internal class RequestTimingMiddleware : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var stopwatch = Stopwatch.StartNew(); // Bắt đầu đo thời gian

			// Tiến hành xử lý request
			await next(context);

			stopwatch.Stop(); // Dừng đo thời gian khi response đã được trả về

			// Ghi log thời gian xử lý
			var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			Console.WriteLine($"Request to {context.Request.Path} took {elapsedMilliseconds}ms");
		}
	}
}