using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Shared;
using System;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class RetrieveOrdersCommand : SkubanaCommand
	{
		public RetrieveOrdersCommand( SkubanaConfig config, DateTime startDateUtc, DateTime endDateUtc, long warehouseId, int page, int limit ) : base( config, SkubanaEndpoint.RetrieveOrdersUrl )
		{
			Condition.Requires( endDateUtc, "endDateUtc" ).IsGreaterThan( startDateUtc );
			Condition.Requires( warehouseId, "warehouseId" ).IsNotEqualTo( 0 );
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 1 );
			Condition.Requires( limit, "limit" ).IsGreaterOrEqual( 1 );

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "modifiedDateFrom", startDateUtc.ConvertDateTimeToStr() },
				{ "modifiedDateTo", endDateUtc.ConvertDateTimeToStr() },
				{ "warehouseId", warehouseId.ToString() },
				{ "page", page.ToString() },
				{ "limit", limit.ToString() }
			};
		}
	}
}