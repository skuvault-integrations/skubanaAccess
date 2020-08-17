using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Netco.Logging;
using SkubanaAccess.Models;
using SkubanaAccess.Throttling;

namespace SkubanaAccess.Services.PurchaseOrders
{
    public interface IPurchaseOrdersService : IDisposable
    {
	    Task< IEnumerable< SkubanaPurchaseOrder > > GetPOsModifiedCreatedInDateRangeAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId, CancellationToken token, Mark mark );
	    Task< Dictionary< long, SkubanaVendor > > GetVendorsByIdsAsync( IEnumerable< long > vendorIds, CancellationToken token, Mark mark );
	    Task< IEnumerable< SkubanaVendor > > GetActiveVendorsAsync( CancellationToken token, Mark mark );
	    Task CreatePurchaseOrdersAsync( IEnumerable< SkubanaPurchaseOrder > purchaseOrders, CancellationToken token, Mark mark );
	    Task< long? > GetVendorProductIdByProductIdVendorIdAsync( long productId, long vendorId, Throttler throttler, CancellationToken token, Mark mark );
	}
}
