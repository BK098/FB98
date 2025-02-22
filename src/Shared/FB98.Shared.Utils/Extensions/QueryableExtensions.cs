using System.Linq.Expressions;

namespace FB98.Shared.Utils.Extensions
{
	public static class QueryableExtensions
	{
		public static IQueryable<T> SortBy<T>(this IQueryable<T> query, string? sortBy, List<string> allowedProperties, bool ascending = true)
			where T : class
		{
			if (string.IsNullOrEmpty(sortBy) || !allowedProperties.Contains(sortBy))
			{
				return query;
			}

			var param = Expression.Parameter(typeof(T), "entity");
			var property = Expression.Property(param, sortBy);
			var sortLambda = Expression.Lambda(property, param);

			var method = ascending ? "OrderBy" : "OrderByDescending";
			var sortedQuery = typeof(Queryable)
				.GetMethods()
				.First(m => m.Name == method && m.GetParameters().Length == 2)
				.MakeGenericMethod(typeof(T), property.Type)
				.Invoke(null, [query, sortLambda]);

			return (IQueryable<T>)sortedQuery!;
		}
	}
}