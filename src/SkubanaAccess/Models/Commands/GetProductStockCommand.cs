using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class GetProductStockCommand : SkubanaCommand
	{
		public GetProductStockCommand( SkubanaConfig config, string sku, long warehouseId ) : base( config, SkubanaEndpoint.RetrieveProductStockUrl )
		{
			Condition.Requires( sku, "sku" ).IsNotNullOrEmpty();
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "sku", sku },
				{ "warehouseId", warehouseId.ToString() }
			};
		}
	}
}