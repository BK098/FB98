using FB98.Shared.Abstractions.Exceptions;
using FB98.Shared.Abstractions.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace FB98.Shared.Infrastructure.Middlewares
{
	internal class ErrorHandlerMiddleware : IMiddleware
	{
		private readonly ConcurrentDictionary<Type, string> _codes = new();
		private readonly IConfiguration _configuration;
		private readonly ILogger<ErrorHandlerMiddleware> _logger;

		public ErrorHandlerMiddleware(
			ILogger<ErrorHandlerMiddleware> logger,
			IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				switch (context.Response.StatusCode)
				{
					case (int)HttpStatusCode.Unauthorized:
						_logger.LogWarning("Unauthorized access detected.");
						await HandleUnauthorizedAsync(context);
						break;
					case (int)HttpStatusCode.Forbidden:
						_logger.LogWarning("Forbidden request detected.");
						await HandleForbiddenAsync(context);
						break;
				}

				await next(context);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);
				await HandleExceptionAsync(context, exception);
			}
		}

		private async Task HandleUnauthorizedAsync(HttpContext context)
		{
			if (context.Response.HasStarted)
			{
				_logger.LogWarning("Response has already started, skipping setting StatusCode.");
				return;
			}

			var response = new ApiResult<string>
			{
				IsSuccess = false,
				StatusCode = (int)HttpStatusCode.Unauthorized,
				Message = "You are not authorized to access this resource.",
				Data = null,
				Errors = new Dictionary<string, List<object>>
				{
					{ "Authorization", ["Invalid or missing token."] }
				},
				Timestamp = DateTime.UtcNow
			};

			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}

		private static async Task HandleForbiddenAsync(HttpContext context)
		{
			var response = new ApiResult<string>
			{
				IsSuccess = false,
				StatusCode = (int)HttpStatusCode.Forbidden,
				Message = "You do not have permission to access this resource.",
				Data = null,
				Errors = new Dictionary<string, List<object>>
				{
					{ "Authorization", ["Access to this resource is forbidden."] }
				},
				Timestamp = DateTime.UtcNow
			};

			context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var statusCode = exception is CustomException ? 400 : 500;
			var response = new ApiResult<string>
			{
				IsSuccess = false,
				StatusCode = statusCode,
				Message = exception is CustomException customException ? customException.Message : "An unexpected error occurred.",
				Data = null,
				Errors = new Dictionary<string, List<object>>
				{
					{ "Exception", [exception.Message] }
				},
				Timestamp = DateTime.UtcNow
			};

			context.Response.StatusCode = statusCode;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}