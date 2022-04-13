using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Services.Orders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class OrdersTests : BaseTest
	{
		private IOrdersService _ordersService;
		private long warehouseId = 107175;
		private readonly DateTime startDateUtc = new DateTime( 2019, 10, 01 ).ToUniversalTime();
		private readonly DateTime endDateUtc = new DateTime( 2021, 10, 20 ).ToUniversalTime();

		[ SetUp ]
		public void Init()
		{
			this._ordersService = new OrdersService( base.Config );
		}

		[ Test ]
		public async Task GetModifiedOrders()
		{
			var orders = await this._ordersService.GetModifiedOrdersAsync( startDateUtc, endDateUtc, warehouseId, CancellationToken.None );

			orders.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetModifiedOrdersBySmallPage()
		{
			base.Config.RetrieveOrdersPageSize = 1;
			var orders = await this._ordersService.GetModifiedOrdersAsync( startDateUtc, endDateUtc, warehouseId, CancellationToken.None );

			orders.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetModifiedOrderWithTrackingNumber()
		{
			var orders = await this._ordersService.GetModifiedOrdersAsync( startDateUtc, endDateUtc, warehouseId, CancellationToken.None );
			var order = orders.FirstOrDefault( f=> f.Id == 113134971 );

			order.ShippingInfo.Shipment.TrackingNumber.Should().Be( "REEW211" );
		}
	}
}