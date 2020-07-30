using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Netco.Logging;
using NUnit.Framework;
using SkubanaAccess.Services.PurchaseOrders;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class PurchaseOrdersTests : BaseTest
	{
		private IPurchaseOrdersService _purchaseOrdersService;
		private long _warehouseId = 107178;

		[ SetUp ]
		public void Init()
		{
			this._purchaseOrdersService = new PurchaseOrdersService( base.Config );
		}

		[ Test ]
		public async Task GetModifiedPurchaseOrdersAsync()
		{
			var endDateUtc = DateTime.UtcNow;
			var startDateUtc = endDateUtc.AddDays( -60 );

			var results = await _purchaseOrdersService.GetModifiedPurchaseOrdersAsync( startDateUtc, endDateUtc, _warehouseId, CancellationToken.None, Mark.Blank() );

			results.Should().NotBeNull();
		}
	}
}
