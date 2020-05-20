using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Orders
{
	public class OrdersService : ServiceBaseWithTokenAuth, IOrdersService
	{
		public OrdersService( SkubanaConfig config ) : base( config )
		{
		}

		public async Task< IEnumerable< SkubanaOrder > > GetModifiedOrdersAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get modified orders request was cancelled", exceptionDetails ) ) );
			}

			var result = new List< SkubanaOrder >();
			var page = 1;

			using( var throttler = new Throttler( 5, 1 ) )
			{
				while( true )
				{
					var command = new RetrieveOrdersCommand( base.Config, startDateUtc, endDateUtc, warehouseId, page, base.Config.RetrieveOrdersPageSize )
					{
						Throttler = throttler
					};
					var pageData = await base.GetAsync< IEnumerable< Order > >( command, token, mark ).ConfigureAwait( false );

					if ( pageData == null || !pageData.Any() )
					{
						break;
					}

					result.AddRange( pageData.Select( o => o.ToSVOrder() ) );
					++page;
				}
			}

			return result;
		}
	}
}