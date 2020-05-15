using SkubanaAccess.Models;
using SkubanaAccess.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Inventory
{
	public interface IInventoryService : IDisposable
	{
		Task CreateProductStock( long productId, int quantity, long warehouseId, CancellationToken token, Mark mark = null );
		Task CreateProductsStock( Dictionary< long, int > productsQuantities, long warehouseId, CancellationToken token, Mark mark = null );
		
		Task< AdjustProductStockQuantityResponse > AdjustProductStockQuantity( string sku, int quantity, long warehouseId, CancellationToken token, Mark mark = null );
		Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantities( Dictionary< string, int > skusQuantities, long warehouseId, CancellationToken token, Mark mark = null );
		
		Task< ProductStock > GetProductStock( string sku, long warehouseId, CancellationToken token, Mark mark = null );
		Task< IEnumerable< ProductStock > > GetProductsStock( long warehouseId, CancellationToken token, Mark mark = null );
	}
}