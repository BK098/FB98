using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FB98.Shared.Infrastructure.Middlewares
{
	internal class RequestTimingMiddleware : IMiddleware
	{
		private readonly ILogger<RequestTimingMiddleware> _logger;

		public RequestTimingMiddleware(
			ILogger<RequestTimingMiddleware> logger)
		{
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var stopwatch = Stopwatch.StartNew();
			var ipAddress = context.Connection.RemoteIpAddress?.ToString();
			if (string.IsNullOrEmpty(ipAddress) && context.Request.Headers.ContainsKey("X-Forwarded-For"))
			{
				ipAddress = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
			}

			await next(context);
			stopwatch.Stop();

			var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			_logger.LogInformation($"Request from IP {ipAddress} to {context.Request.Path} took {elapsedMilliseconds}ms");
		}
	}
}