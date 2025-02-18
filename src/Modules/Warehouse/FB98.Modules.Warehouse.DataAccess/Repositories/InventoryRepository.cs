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
		public InventoryRepository(WarehouseModuleDbContext context) : base(context) { }

		public async Task AddStockAsync(Guid productId, int quantity, bool isLimited)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);

			if (inventory == null)
			{
				inventory = new Inventory
				{
					ProductId = productId,
					Quantity = quantity,
					IsLimited = isLimited
				};
				inventory.SetCreatedAt();
				Create(inventory);
			}
			else
			{
				if (inventory.IsLimited)
				{
					inventory.Quantity += quantity;
				}
				inventory.SetUpdatedAt();
			}

			await RecordTransaction(productId, inventory.Id, quantity, IMPORT, inventory.IsLimited);
			await _context.SaveChangesAsync();
		}

		public async Task<Inventory?> GetStock(Guid? productId)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			return inventory;
		}

		private async Task RecordTransaction(Guid productId, Guid inventoryId, int quantityChange, string transactionType, bool isLimited)
		{
			var transaction = new InventoryTransaction
			{
				InventoryId = inventoryId,
				ProductId = productId,
				QuantityChange = quantityChange,
				TransactionType = transactionType,
				IsLimited = isLimited
			};
			var isAdded = _context.InventoryTransactions.Any(x => x.ProductId == productId);
			if (isAdded)
			{
				transaction.SetCreatedAt();
			}
			transaction.SetUpdatedAt();

			_context.InventoryTransactions.Add(transaction);
			await _context.SaveChangesAsync();
		}

		public async Task ReduceStock(Guid productId, int quantity)
		{
			var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
			if (inventory == null || inventory.Quantity < quantity)
			{
				throw new InvalidOperationException("Not enough stock available.");
			}

			if (inventory.IsLimited)
			{
				if (inventory.Quantity < quantity)
				{
					throw new InvalidOperationException("Not enough stock available.");
				}
				inventory.Quantity -= quantity;
			}
			inventory.SetUpdatedAt();

			await RecordTransaction(productId, inventory.Id, -quantity, EXPORT, inventory.IsLimited);
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
	}
}
