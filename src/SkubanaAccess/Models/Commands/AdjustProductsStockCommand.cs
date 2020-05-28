using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Models.RequestBody;
using SkubanaAccess.Throttling;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class AdjustProductsStockCommand : SkubanaCommand
	{
		private const int maxQuantity = 100 * 1000 * 1000;

		public AdjustProductsStockCommand( SkubanaConfig config, IEnumerable< SkubanaProductStock > skusQuantities, long warehouseId ) : base( config, SkubanaEndpoint.AdjustProductStockUrl )
		{
			Condition.Requires( skusQuantities, "skusQuantities" ).IsNotNull().IsNotEmpty();
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );

			var body = new List< AdjustProductStockRequestBodyItem >();
			foreach( var skuQuantity in skusQuantities )
			{
				body.Add( new AdjustProductStockRequestBodyItem() 
				{
					MasterSku = skuQuantity.ProductSku,
					OnHandQuantity = skuQuantity.OnHandQuantity > maxQuantity ? maxQuantity : skuQuantity.OnHandQuantity,
					WarehouseId = warehouseId,
					StockLocationName = skuQuantity.LocationName
				} );
			}

			this.Payload = body;
		}
	}
}