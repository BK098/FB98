using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.ShoppingList.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<ShoppingListModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(ShoppingListModuleDbContext context) : base(context)
		{
		}
	}
}