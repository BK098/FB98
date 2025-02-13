using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FB98.Shared.Infrastructure.Postgres
{
	public static class Extensions
	{
		internal static IServiceCollection AddPostgres(this IServiceCollection services)
		{
			var options = services.GetOptions<PostgresOptions>("postgres");
			services.AddSingleton(options);
			try
			{
				using var connection = new Npgsql.NpgsqlConnection(options.ConnectionString);
				connection.Open();
				Console.WriteLine("Database connected successfully!");
				connection.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Database connection failed: {ex.Message}");
			}
			return services;
		}

		public static IServiceCollection AddPostgres<T>(this IServiceCollection services) where T : DbContext
		{
			var options = services.GetOptions<PostgresOptions>("postgres");
			services.AddDbContext<T>(x =>
				x.UseNpgsql(options.ConnectionString,
				sqlOptions => sqlOptions.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: TimeSpan.FromSeconds(30),
					errorCodesToAdd: null
				))
				.EnableSensitiveDataLogging()
				.LogTo(Console.WriteLine, LogLevel.Information)
			);
			using var scope = services.BuildServiceProvider().CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<T>();
			dbContext.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS unaccent;");
			dbContext.Database.Migrate();

			return services;
		}
	}
}