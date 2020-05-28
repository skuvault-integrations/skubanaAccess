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
		Task CreateProductStockIn3PLWarehouse( long productId, int quantity, long warehouseId, CancellationToken token, Mark mark = null );
		Task CreateProductsStockIn3PLWarehouse( Dictionary< long, int > productsQuantities, long warehouseId, CancellationToken token, Mark mark = null );
		
		Task CreateProductStockInInHouseWarehouse( long productId, int quantity, long warehouseId, string locationName, CancellationToken token, Mark mark = null );
		Task CreateProductsStockInInHouseWarehouse( IEnumerable< SkubanaProductStock > productsStocks, long warehouseId, CancellationToken token, Mark mark = null );
		
		Task< AdjustProductStockQuantityResponse > AdjustProductStockQuantityTo3PLWarehouse( string sku, int quantity, long warehouseId, CancellationToken token, Mark mark = null );
		Task< AdjustProductStockQuantityResponse > AdjustProductStockQuantityToInHouseWarehouse( string sku, int quantity, long warehouseId, string locationName, CancellationToken token, Mark mark = null );
		
		Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantitiesTo3PLWarehouse( Dictionary< string, int > skusQuantities, long warehouseId, CancellationToken token, Mark mark = null );
		Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantitiesToInHouseWarehouse( IEnumerable< SkubanaProductStock > skusQuantitiesByLocation, long warehouseId, CancellationToken token, Mark mark = null );

		Task< SkubanaProductStock > GetProductStock( string sku, long warehouseId, CancellationToken token, Mark mark = null );
		Task< IEnumerable< SkubanaProductStock > > GetDetailedProductStock( string sku, long warehouseId, CancellationToken token, Mark mark = null );
		Task< IEnumerable< SkubanaProductStock > > GetProductsStock( long warehouseId, CancellationToken token, Mark mark = null );
	}
}