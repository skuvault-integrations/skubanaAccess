using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Netco.Logging;
using SkubanaAccess.Models;

namespace SkubanaAccess.Services.PurchaseOrders
{
    public interface IPurchaseOrdersService : IDisposable
    {
	    Task< IEnumerable< SkubanaPurchaseOrder > > GetModifiedPurchaseOrdersAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId, CancellationToken token, Mark mark );
    }
}
