using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Services.Orders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class OrdersTests : BaseTest
	{
		private IOrdersService _ordersService;
		private long _warehouseId = 107178;

		[ SetUp ]
		public void Init()
		{
			this._ordersService = new OrdersService( base.Config );
		}

		[ Test ]
		public async Task GetModifiedOrders()
		{
			var orders = await this._ordersService.GetModifiedOrdersAsync( DateTime.UtcNow.AddMonths( -1 ), DateTime.UtcNow, _warehouseId, CancellationToken.None );

			orders.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetModifiedOrdersBySmallPage()
		{
			base.Config.RetrieveOrdersPageSize = 1;
			var orders = await this._ordersService.GetModifiedOrdersAsync( DateTime.UtcNow.AddMonths( -1 ), DateTime.UtcNow, _warehouseId, CancellationToken.None );

			orders.Should().NotBeNullOrEmpty();
		}
	}
}