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

		public async Task< IEnumerable< SkubanaPurchaseOrder > > GetPOsModifiedCreatedInDateRangeAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId,
			CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get purchase orders request was cancelled", exceptionDetails ) ) );
			}

			var modifiedPurchaseOrders = await GetPOsModifiedInDateRangeAsync( startDateUtc, endDateUtc, warehouseId, token, skubanaMark );
			//Have to get both created & modified, because the modifiedDate filter won't return new POs until they've been modified
			//	New POs have a null Modified Date upon creation, until they are modified
			var createdPurchaseOrders = await GetPOsCreatedInDateRangeAsync( startDateUtc, endDateUtc, warehouseId, token, skubanaMark );

			var purchaseOrders = modifiedPurchaseOrders.Concat( createdPurchaseOrders ).Distinct( new PurchaseOrderComparer() ).ToList();

			return await FillPurchaseOrdersVendors( purchaseOrders, token, mark );
		}

		private async Task< IEnumerable< SkubanaPurchaseOrder > > GetPOsModifiedInDateRangeAsync( DateTime startDateUtc, DateTime endDateUtc,
			long warehouseId, CancellationToken token, Shared.Mark mark )
		{
			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get purchase orders modified in date range request was cancelled", exceptionDetails ) ) );
			}

			var purchaseOrders = new List< SkubanaPurchaseOrder >();
			var page = 1;

			using ( var throttler = new Throttler( 5, 1 ) )
			{
				while ( true )
				{
					var command = new RetrievePOsModifiedBetweenDatesCommand( base.Config, startDateUtc, endDateUtc, page, base.Config.RetrievePurchaseOrdersPageSize )
					{
						Throttler = throttler
					};
					var pageData = await base.GetAsync< IEnumerable< PurchaseOrder > >( command, token, mark ).ConfigureAwait( false );

					if ( pageData == null || !pageData.Any() )
					{
						break;
					}

					purchaseOrders.AddRange( pageData.FilterByWarehouse( warehouseId ).Select( o => o.ToSVPurchaseOrder() ) );
					++page;
				}
			}

			return purchaseOrders;
		}

		private async Task< IEnumerable< SkubanaPurchaseOrder > > GetPOsCreatedInDateRangeAsync( DateTime startDateUtc, DateTime endDateUtc,
			long warehouseId, CancellationToken token, Shared.Mark mark )
		{
			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get purchase orders created in date range request was cancelled", exceptionDetails ) ) );
			}

			var purchaseOrders = new List< SkubanaPurchaseOrder >();
			var page = 1;

			using ( var throttler = new Throttler( 5, 1 ) )
			{
				while ( true )
				{
					var command = new RetrievePOsCreatedBetweenDatesCommand( base.Config, startDateUtc, endDateUtc, page, base.Config.RetrievePurchaseOrdersPageSize )
					{
						Throttler = throttler
					};
					var pageData = await base.GetAsync< IEnumerable< PurchaseOrder > >( command, token, mark ).ConfigureAwait( false );

					if ( pageData == null || !pageData.Any() )
					{
						break;
					}

					purchaseOrders.AddRange( pageData.FilterByWarehouse( warehouseId ).Select( o => o.ToSVPurchaseOrder() ) );
					++page;
				}
			}

			return purchaseOrders;
		}

		private async Task< IEnumerable< SkubanaPurchaseOrder > > FillPurchaseOrdersVendors( IEnumerable< SkubanaPurchaseOrder > purchaseOrders, CancellationToken token, Mark mark )
		{
			var vendorIds = purchaseOrders.Select( p => p.VendorId );

			var vendorIdNames = await this.GetVendorsNamesByIdsAsync( vendorIds, token, mark );

			foreach ( var purchaseOrder in purchaseOrders )
			{
				string vendorName;

				if ( vendorIdNames.TryGetValue( purchaseOrder.VendorId, out vendorName ) )
				{
					purchaseOrder.VendorName = vendorName;
				}
			}

			return purchaseOrders;
		}

		public async Task< Dictionary< long, string > > GetVendorsNamesByIdsAsync( IEnumerable< long > vendorIds, CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get vendor names by ids request was cancelled", exceptionDetails ) ) );
			}

			var vendorIdNames = new Dictionary< long, string >();

			using ( var throttler = new Throttler( 5, 1 ) )
			{
				foreach ( var vendorId in vendorIds )
				{
					var command = new GetVendorByIdCommand( base.Config, vendorId )
					{
						Throttler = throttler
					};

					Vendor vendor;
					try
					{
						vendor = await base.GetAsync< Vendor >( command, token, skubanaMark ).ConfigureAwait( false );
					}
					catch
					{
						continue;
					}

					if ( !string.IsNullOrWhiteSpace( vendor?.Name ?? "" ) && !vendorIdNames.ContainsKey( vendorId ) )
					{
						vendorIdNames.Add( vendorId , vendor.Name );
					}
				}
			}

			return vendorIdNames;
		}
	}
}
