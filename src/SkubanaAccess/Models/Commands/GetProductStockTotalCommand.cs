using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Throttling;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class GetProductStockTotalCommand : SkubanaCommand
	{
		public GetProductStockTotalCommand( SkubanaConfig config, string sku, long warehouseId ) : base( config, SkubanaEndpoint.GetProductsStocksTotalUrl )
		{
			Condition.Requires( sku, "sku" ).IsNotNullOrEmpty();
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "sku", sku },
				{ "warehouseId", warehouseId.ToString() }
			};
			this.Throttler = new Throttler( 10, 1 );
		}
	}
}