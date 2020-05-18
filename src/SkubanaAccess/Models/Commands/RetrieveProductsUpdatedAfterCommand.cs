using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class RetrieveProductsUpdatedAfterCommand : SkubanaCommand
	{
		public RetrieveProductsUpdatedAfterCommand( SkubanaConfig config, DateTime updatedAfterUtc, int page, int limit ) : base( config, SkubanaEndpoint.RetrieveProductsUrl )
		{
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 1 );
			Condition.Requires( limit, "limit" ).IsGreaterOrEqual( 1 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "createdDateFrom", updatedAfterUtc.ToString( "yyyy-MM-ddTHH:mm:ssZ" ) },
				{ "page", page.ToString() },
				{ "limit", limit.ToString() }
			};
		}
	}
}
