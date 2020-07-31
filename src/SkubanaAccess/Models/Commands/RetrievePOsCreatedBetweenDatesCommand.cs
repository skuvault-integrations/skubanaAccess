using System;
using System.Collections.Generic;
using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Shared;

namespace SkubanaAccess.Models.Commands
{
	public class RetrievePOsCreatedBetweenDatesCommand : SkubanaCommand
	{
		public RetrievePOsCreatedBetweenDatesCommand( SkubanaConfig config, DateTime startDateUtc, DateTime endDateUtc, int page, int limit ) : base( config, SkubanaEndpoint.RetrievePurchaseOrdersUrl )
		{		
			Condition.Requires( endDateUtc, "endDateUtc" ).IsGreaterThan( startDateUtc );
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 1 );
			Condition.Requires( limit, "limit" ).IsGreaterOrEqual( 1 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "createdDateFrom", startDateUtc.ConvertDateTimeToStr() },
				{ "createdDateTo", endDateUtc.ConvertDateTimeToStr() },
				{ "page", page.ToString() },
				{ "limit", limit.ToString() }
			};
		}						  
	}
}
