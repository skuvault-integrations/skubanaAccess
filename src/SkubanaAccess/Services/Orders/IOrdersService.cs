using SkubanaAccess.Models;
using SkubanaAccess.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Orders
{
	public interface IOrdersService : IDisposable
	{
		Task< IEnumerable< SkubanaOrder > > GetModifiedOrdersAsync( DateTime startDateUtc, DateTime endDateUtc, long warehouseId, CancellationToken token, Mark mark = null );
	}
}