using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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
				using var connection = new NpgsqlConnection(options.ConnectionString);
				connection.Open();
				Console.WriteLine(@"Database connected successfully!");
				connection.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine($@"Database connection failed: {ex.Message}");
			}

			return services;
		}

		public static IServiceCollection AddPostgres<T>(this IServiceCollection services) where T : DbContext
		{
			var options = services.GetOptions<PostgresOptions>("postgres");
			services.AddDbContext<T>(x =>
					x.UseNpgsql(options.ConnectionString,
						sqlOptions => sqlOptions.EnableRetryOnFailure(
							5,
							TimeSpan.FromSeconds(30),
							null
						))
#if DEBUG
			//.EnableSensitiveDataLogging()
			//.LogTo(Console.WriteLine, LogLevel.Information)
#endif
			);
			using var scope = services.BuildServiceProvider().CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<T>();
			dbContext.Database.Migrate();
			dbContext.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS unaccent;");
			dbContext.Database.ExecuteSqlRaw("SET TIME ZONE 'Asia/Ho_Chi_Minh';");

			return services;
		}
	}
}