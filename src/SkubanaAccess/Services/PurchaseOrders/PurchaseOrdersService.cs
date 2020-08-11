using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Models.RequestBody;
using SkubanaAccess.Models.Response;
using SkubanaAccess.Services.Products;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;
using Mark = Netco.Logging.Mark;

namespace SkubanaAccess.Services.PurchaseOrders
{
	public class PurchaseOrdersService : ServiceBaseWithTokenAuth, IPurchaseOrdersService
	{
		private readonly IProductsService _productsService;

		public PurchaseOrdersService( SkubanaConfig config, IProductsService productsService ) : base( config )
		{
			this._productsService = productsService;
		}

		/// <summary>
		/// Get purchase orders in warehouseId & created/modified between startDateUtc & endDateUtc
		/// </summary>
		/// <param name="startDateUtc"></param>
		/// <param name="endDateUtc"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< SkubanaPurchaseOrder > > GetPOsModifiedCreatedInDateRangeAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId,
			CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Get purchase orders request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var modifiedPurchaseOrders = await GetPOsModifiedInDateRangeAsync( startDateUtc, endDateUtc, warehouseId, token, skubanaMark );
			//Have to get both created & modified, because the modifiedDate filter won't return new POs until they've been modified
			//	New POs have a null Modified Date upon creation, until they are modified
			var createdPurchaseOrders = await GetPOsCreatedInDateRangeAsync( startDateUtc, endDateUtc, warehouseId, token, skubanaMark );

			var purchaseOrders = modifiedPurchaseOrders.Concat( createdPurchaseOrders ).Distinct( new PurchaseOrderComparer() ).ToList();

			return await FillPurchaseOrdersVendorsAsync( purchaseOrders, token, mark );
		}

		private async Task< IEnumerable< SkubanaPurchaseOrder > > GetPOsModifiedInDateRangeAsync( DateTime startDateUtc, DateTime endDateUtc,
			long warehouseId, CancellationToken token, Shared.Mark mark )
		{
			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Get purchase orders modified in date range request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
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
				var exception = new SkubanaException( string.Format( "{0}. Get purchase orders created in date range request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
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

		private async Task< IEnumerable< SkubanaPurchaseOrder > > FillPurchaseOrdersVendorsAsync( IEnumerable< SkubanaPurchaseOrder > purchaseOrders, CancellationToken token, Mark mark )
		{
			var vendorIds = purchaseOrders.Select( p => p.VendorId ).Distinct();
			var vendors = await this.GetVendorsByIdsAsync( vendorIds, token, mark );

			foreach ( var purchaseOrder in purchaseOrders )
			{
				SkubanaVendor vendor;
				if ( vendors.TryGetValue( purchaseOrder.VendorId, out vendor ) )
				{
					purchaseOrder.Vendor = vendor;
				}
			}

			return purchaseOrders;
		}

		/// <summary>
		/// Create purchase orders
		/// </summary>
		/// <param name="purchaseOrders"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task CreatePurchaseOrdersAsync( IEnumerable< SkubanaPurchaseOrder > purchaseOrders, CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Create Purchase Orders request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var posProductIdsBySkus = await GetProductIdsBySkusAsync( purchaseOrders, token, skubanaMark );
			using( var throttler = new Throttler( 1, 30 ) )	//TODO GUARD-709 Seems slow. Copied from CreateProductsStock. Pending question for Bulat
			{
				foreach ( var purchaseOrder in purchaseOrders )
				{
					var vendorId = purchaseOrder.Vendor?.VendorId;
					if ( vendorId == null )
					{
						var methodCallInfo = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
						SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Purchase order number {1} has no Vendor or Vendor > VendorId", methodCallInfo, purchaseOrder.CustomPurchaseOrderNumber ) ) );
						continue;
					}

					var vendorProductIdsBySku = await GetPurchaseOrderVendorProductIdsAsync( purchaseOrder.Items, vendorId.Value, posProductIdsBySkus, token, mark );
					var command = new CreatePurchaseOrderCommand( base.Config, purchaseOrder, vendorProductIdsBySku )
					{
						Throttler = throttler
					};
					var response = await base.PutAsync< SkubanaResponse< CreatePurchaseOrderRequestBody > >( command, token, skubanaMark ).ConfigureAwait( false );

					if ( response.Errors != null && response.Errors.Any() )
					{
						var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
						var exception = new SkubanaException( string.Format( "{0}. Create Purchase Orders request for purchase order number {1} failed. Error: {2}", exceptionDetails, purchaseOrder.CustomPurchaseOrderNumber, response.Errors.ToJson() ) );
						SkubanaLogger.LogTraceException( exception );
					}
				}
			}
		}

		private async Task< Dictionary< string, long > > GetProductIdsBySkusAsync( IEnumerable< SkubanaPurchaseOrder > purchaseOrders, CancellationToken token, Shared.Mark skubanaMark )
		{
			var uniquePoItemSkus = purchaseOrders.SelectMany( p => p.Items ).Select( i => i.MasterSku ).Distinct();
			var posProducts = await _productsService.GetProductsBySkus( uniquePoItemSkus, token, skubanaMark );
			return posProducts?.ToDictionary( x => x.Sku, x => x.Id ) ?? new Dictionary< string, long >();
		}

		private async Task< Dictionary< string, long > > GetPurchaseOrderVendorProductIdsAsync( IEnumerable< SkubanaPurchaseOrderItem > poItems, 
			long vendorId, IDictionary< string, long > productIdsBySkus, CancellationToken token, Mark mark )
		{
			var vendorProductIdsBySku = new Dictionary< string, long >();
			foreach ( var poItem in poItems )
			{
				var sku = poItem.MasterSku;
				var vendorProductId = await GetVendorProductIdByProductIdVendorIdAsync(productIdsBySkus[ sku ], vendorId, token, mark);
				if ( !vendorProductIdsBySku.ContainsKey( sku ) && vendorProductId != null )
				{
					vendorProductIdsBySku.Add( sku, vendorProductId.Value );
				}
			}

			return vendorProductIdsBySku;
		}

		/// <summary>
		/// Get VendorProductId by productId & vendorId. Null if not found
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="vendorId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< long? > GetVendorProductIdByProductIdVendorIdAsync( long productId, long vendorId,
			CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var methodCallInfo = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. GetVendorProductByProductIdVendorIdAsync request for productId {1}, vendorId {2} was cancelled", methodCallInfo, productId, vendorId ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var command = new GetVendorProductByProductIdVendorIdCommand( this.Config, productId, vendorId );

			var response = await base.GetAsync< IEnumerable< VendorProduct > >( command, token, skubanaMark ).ConfigureAwait( false );

			var firstVendorProduct = response?.FirstOrDefault();
			if ( firstVendorProduct == null )
			{
				var methodCallInfo = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Can't find vendor product for productId {1} and vendorId {2}", methodCallInfo, productId, vendorId ) ) );
			}

			return firstVendorProduct?.VendorProductId;
		}

		/// <summary>
		/// Get vendors by vendorIds
		/// </summary>
		/// <param name="vendorIds"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< Dictionary< long, SkubanaVendor > > GetVendorsByIdsAsync( IEnumerable< long > vendorIds, CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var methodCallInfo = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Get vendors by ids request was cancelled", methodCallInfo ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var vendors = new Dictionary< long, SkubanaVendor >();

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
					catch ( Exception ex )
					{
						var methodCallInfo = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
						var exception = new SkubanaException( string.Format( "{0}. Unable to get vendor {1}. Error: {2}", methodCallInfo, vendorId, ex.ToJson() ) );
						SkubanaLogger.LogTraceException( exception );
						continue;
					}

					if ( vendor != null )
					{
						vendors.Add( vendorId , vendor.ToSvVendor() );
					}
				}
			}

			return vendors;
		}

		/// <summary>
		/// Get all vendors
		/// </summary>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< SkubanaVendor > > GetVendorsAsync( CancellationToken token, Mark mark )
		{
			var skubanaMark = new Shared.Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Get Vendors request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var result = new List< SkubanaVendor >();
			var page = 1;

			using( var throttler = new Throttler( 5, 1 ) )
			{
				while( true )
				{
					var command = new GetVendorsCommand( base.Config, page, base.Config.RetrieveVendorsPageSize )
					{
						Throttler = throttler
					};
					var pageData = await base.GetAsync< IEnumerable< Vendor > >( command, token, skubanaMark ).ConfigureAwait( false );

					if ( pageData == null || !pageData.Any() )
					{
						break;
					}

					result.AddRange( pageData.Select( v => v.ToSvVendor() ) );
					++page;
				}
			}

			return result;
		}
	}
}
