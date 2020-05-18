using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class RetrieveProductsStocksTotalCommand : SkubanaCommand
	{
		public RetrieveProductsStocksTotalCommand( SkubanaConfig config, long warehouseId, int page, int limit ) : base( config, SkubanaEndpoint.GetProductsStocksTotalUrl )
		{
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 1 );
			Condition.Requires( limit, "limit" ).IsGreaterOrEqual( 1 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "warehouseId", warehouseId.ToString() },
				{ "page", page.ToString() },
				{ "limit", limit.ToString() }
			};
		}
	}
}