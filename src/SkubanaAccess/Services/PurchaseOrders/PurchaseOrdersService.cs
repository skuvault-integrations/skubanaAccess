using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;
using Mark = Netco.Logging.Mark;

namespace SkubanaAccess.Services.PurchaseOrders
{
	public class PurchaseOrdersService : ServiceBaseWithTokenAuth, IPurchaseOrdersService
	{
		public PurchaseOrdersService( SkubanaConfig config ) : base( config )
		{
		}

		public async Task< IEnumerable< SkubanaPurchaseOrder > > GetModifiedPurchaseOrdersAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId,
			CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.ToString() );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get modified purchase orders request was cancelled", exceptionDetails ) ) );
			}

			var result = new List< SkubanaPurchaseOrder >();
			var page = 1;

			using ( var throttler = new Throttler( 5, 1 ) )
			{
				while ( true )
				{
					var command = new RetrievePurchaseOrdersCommand( base.Config, startDateUtc, endDateUtc, page, base.Config.RetrievePurchaseOrdersPageSize )
					{
						Throttler = throttler
					};
					var pageData = await base.GetAsync< IEnumerable< PurchaseOrder > >( command, token, skubanaMark ).ConfigureAwait( false );

					if ( pageData == null || !pageData.Any() )
					{
						break;
					}

					result.AddRange( pageData.FilterByWarehouse( warehouseId ).Select( o => o.ToSVPurchaseOrder() ) );
					++page;
				}
			}

			return result;
		}
	}
}
