using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;
using System.Collections.Concurrent;

namespace FB98.Modules.Catalog.Application.ProductManagement.Events
{
	public sealed class StockResponseEventHandler : IEventHandler<StockResponseEvent>
	{
		private static readonly ConcurrentDictionary<Guid, TaskCompletionSource<int>> _pendingRequests = new();
		private static readonly ConcurrentDictionary<Guid, int> _cachedResponses = new();

		public async Task HandleAsync(StockResponseEvent notification)
		{
			await Task.Yield();

			if (_pendingRequests.TryRemove(notification.ProductId, out var tcs))
			{
				tcs.SetResult(notification.Quantity);
				//Console.WriteLine($"[Catalog] StockResponseEventHandler - TaskCompletionSource Resolved for ProductId: {notification.ProductId}");
			}
			else
			{
				_cachedResponses[notification.ProductId] = notification.Quantity;
				//Console.WriteLine($"[Catalog] StockResponseEventHandler - Cached StockResponseEvent for ProductId: {notification.ProductId}");
			}
		}

		public static Task<int> WaitForStockResponse(Guid productId)
		{
			if (_cachedResponses.TryRemove(productId, out int cachedQuantity))
			{
				Console.WriteLine($"[Catalog] Using Cached StockResponseEvent for ProductId: {productId} with Quantity: {cachedQuantity}");
				return Task.FromResult(cachedQuantity);
			}
			var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
			_pendingRequests[productId] = tcs;
			return tcs.Task;
		}
	}
}
