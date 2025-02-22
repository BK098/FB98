using Microsoft.EntityFrameworkCore;

namespace FB98.Shared.Infrastructure.Paging
{
	public class PaginatedResult<T> where T : class
	{
		public List<T> Items { get; set; }
		public int PageIndex { get; set; }
		public int TotalPages { get; private set; }
		public int TotalCount { get; set; }
		public int PageSize { get; set; }
		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;

		public PaginatedResult(List<T> items, int pageIndex, int pageSize, int count)
		{
			Items = items;
			TotalCount = count;
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		}

		public static async Task<PaginatedResult<T>> CreateAsync(
			IQueryable<T> source,
			int pageIndex,
			int pageSize,
			CancellationToken cancellation)
		{
			var count = await source.CountAsync(cancellation);
			var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellation);
			return new PaginatedResult<T>(items, pageIndex, pageSize, count);
		}
	}
}