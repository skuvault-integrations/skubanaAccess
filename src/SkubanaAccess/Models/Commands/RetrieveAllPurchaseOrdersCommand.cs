using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class RetrieveAllPurchaseOrdersCommand : SkubanaCommand
	{
		public RetrieveAllPurchaseOrdersCommand( SkubanaConfig config, int page, int limit ) : base( config, SkubanaEndpoint.RetrievePurchaseOrdersUrl )
		{
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 1 );
			Condition.Requires( limit, "limit" ).IsGreaterOrEqual( 1 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "page", page.ToString() },
				{ "limit", limit.ToString() }
			};
		}
	}
}
