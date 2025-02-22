using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Modules.Warehouse.DataAccess.Data;
using FB98.Modules.Warehouse.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Warehouse.DataAccess.Repositories
{
	public class InventoryRepository : BaseRepository<Inventory, WarehouseModuleDbContext>, IInventoryRepository
	{
		private const string IMPORT = "import";
		private const string EXPORT = "export";
		private const string RESERVE = "reserve";
		private const string RELEASE = "release";

		public InventoryRepository(WarehouseModuleDbContext context) : base(context)
		{
		}

		public async Task AddStockAsync(Guid productId, int quantity, bool isLimited)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			if (inventory == null)
			{
				inventory = new Inventory
				{
					ProductId = productId,
					Quantity = isLimited ? quantity : 0,
					IsLimited = isLimited
				};
				inventory.SetCreatedAt();
				await CreateAsync(inventory);
			}
			else
			{
				if (inventory.IsLimited)
				{
					inventory.Quantity += quantity;
				}

				inventory.SetUpdatedAt();
			}

			await RecordTransaction(null, productId, inventory.Id, quantity, IMPORT, inventory.IsLimited);
			await _context.SaveChangesAsync();
		}

		public async Task<Inventory?> GetStock(Guid? productId)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			return inventory;
		}

		public async Task ReduceStock(Guid productId, int quantity)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			if (inventory == null)
			{
				throw new InvalidOperationException("Invalid");
			}

			if (inventory.IsLimited)
			{
				if (inventory.Quantity < quantity)
				{
					throw new InvalidOperationException("Not enough stock available.");
				}

				inventory.Quantity -= quantity;
			}

			inventory.SetCreatedAt();
			inventory.SetUpdatedAt();

			await RecordTransaction(null, productId, inventory.Id, -quantity, EXPORT, inventory.IsLimited);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> Exists(Guid productId)
		{
			return await _context.Inventories.AnyAsync(x => x.ProductId == productId);
		}

		public async Task<bool> RemoveProduct(Guid productId)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			if (inventory == null)
			{
				return false;
			}

			_context.Inventories.Remove(inventory);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task ReserveStock(Guid orderId, Guid productId, int quantity)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			if (inventory == null)
			{
				throw new InvalidOperationException("Invalid");
			}

			if (inventory.IsLimited)
			{
				if (inventory.Quantity < quantity)
				{
					throw new InvalidOperationException("Not enough stock available.");
				}

				inventory.Quantity -= quantity;
				inventory.ReservedQuantity += quantity;
			}
			inventory.SetUpdatedAt();
			await RecordTransaction(orderId, productId, inventory.Id, quantity, RESERVE, inventory.IsLimited);

			await _context.SaveChangesAsync();
		}

		public async Task ReleaseStock(Guid orderId)
		{
			var transactions = await _context.InventoryTransactions
				.Where(x => x.OrderId == orderId && x.TransactionType == RESERVE)
				.ToListAsync();
			foreach (var transaction in transactions)
			{
				var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == transaction.ProductId);
				if (inventory == null || !inventory.IsLimited)
				{
					continue;
				}

				inventory.Quantity += transaction.QuantityChange;
				inventory.ReservedQuantity -= transaction.QuantityChange;
				inventory.SetUpdatedAt();
				await RecordTransaction(orderId, transaction.ProductId, inventory.Id, transaction.QuantityChange, RELEASE, inventory.IsLimited);
			}
			await _context.SaveChangesAsync();
		}

		private Task RecordTransaction(Guid? orderId, Guid productId, Guid inventoryId, int quantityChange, string transactionType, bool isLimited)
		{
			var transaction = new InventoryTransaction
			{
				OrderId = orderId,
				InventoryId = inventoryId,
				ProductId = productId,
				QuantityChange = isLimited ? quantityChange : 0,
				TransactionType = transactionType,
				IsLimited = isLimited
			};

			transaction.SetCreatedAt();
			transaction.SetUpdatedAt();

			_context.InventoryTransactions.Add(transaction);
			return Task.CompletedTask;
		}

		public async Task StockDeduct(Guid orderId)
		{
			var transactions = await _context.InventoryTransactions
				.Where(x => x.OrderId == orderId && x.TransactionType == RESERVE)
				.ToListAsync();
			foreach (var transaction in transactions)
			{
				var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == transaction.ProductId);
				if (inventory == null || !inventory.IsLimited)
				{
					transaction.TransactionType = EXPORT;
					_context.InventoryTransactions.Update(transaction);
					continue;
				}

				inventory.ReservedQuantity -= transaction.QuantityChange;
				inventory.SetUpdatedAt();

				await RecordTransaction(orderId, transaction.ProductId, inventory.Id, -transaction.QuantityChange, EXPORT, inventory.IsLimited);
			}

			await _context.SaveChangesAsync();
		}
	}
}