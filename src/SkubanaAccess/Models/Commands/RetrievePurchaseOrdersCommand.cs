using System;
using System.Collections.Generic;
using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Shared;

namespace SkubanaAccess.Models.Commands
{
	public class RetrievePurchaseOrdersCommand : SkubanaCommand
	{
		public RetrievePurchaseOrdersCommand( SkubanaConfig config, DateTime startDateUtc, DateTime endDateUtc, int page, int limit ) : base( config, SkubanaEndpoint.RetrievePurchaseOrdersUrl )
		{		
			Condition.Requires( endDateUtc, "endDateUtc" ).IsGreaterThan( startDateUtc );
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 1 );
			Condition.Requires( limit, "limit" ).IsGreaterOrEqual( 1 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "modifiedDateFrom", startDateUtc.ConvertDateTimeToStr() },
				{ "modifiedDateTo", endDateUtc.ConvertDateTimeToStr() },
				{ "page", page.ToString() },
				{ "limit", limit.ToString() }
			};
		}						  
	}
}
