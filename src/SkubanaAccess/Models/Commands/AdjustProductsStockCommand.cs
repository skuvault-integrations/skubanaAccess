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

		public AdjustProductsStockCommand( SkubanaConfig config, Dictionary< string, int > skusQuantities, long warehouseId ) : base( config, SkubanaEndpoint.AdjustProductStockUrl )
		{
			Condition.Requires( skusQuantities, "skusQuantities" ).IsNotNull().IsNotEmpty();
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );

			var body = new List< AdjustProductStockRequestBodyItem >();
			foreach( var skuQuantity in skusQuantities )
			{
				body.Add( new AdjustProductStockRequestBodyItem()
				{
					MasterSku = skuQuantity.Key,
					OnHandQuantity = skuQuantity.Value > maxQuantity ? maxQuantity : skuQuantity.Value,
					WarehouseId = warehouseId
				} );
			}

			this.Payload = body;
		}
	}
}