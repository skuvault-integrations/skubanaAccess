using System;
using System.Collections.Generic;
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
		public async Task GetPOsModifiedCreatedInDateRangeAsync()
		{
			var endDateUtc = DateTime.UtcNow;
			var startDateUtc = endDateUtc.AddDays( -60 );

			var results = await _purchaseOrdersService.GetPOsModifiedCreatedInDateRangeAsync( startDateUtc, endDateUtc, _warehouseId, CancellationToken.None, Mark.Blank() );

			results.Should().NotBeNull();
		}

		[ Test ]
		public async Task GetVendorsNamesByIdsAsync()
		{
			const long vendorId = 755;
			const long vendorIdDoestExist = 9999999;

			var vendorIds = new List< long >
			{
				vendorId,
				vendorIdDoestExist
			};

			var results = await _purchaseOrdersService.GetVendorsNamesByIdsAsync( vendorIds, CancellationToken.None, Mark.Blank() );

			results.Count.Should().Be( 1 );
			results[ vendorId ].Should().Be( "Amazon" );
		}
	}
}
