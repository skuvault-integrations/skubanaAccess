﻿using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Services.Global;
using System.Threading;
using System.Threading.Tasks;
using Netco.Logging;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class GlobalTests : BaseTest
	{
		private IGlobalService _globalService;

		[ SetUp ]
		public void Init()
		{
			this._globalService = new GlobalService( base.Config );
		}

		[ Test ]
		public async Task ListWarehouses()
		{
			var warehouses = await this._globalService.ListWarehouses( CancellationToken.None );

			warehouses.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetWarehouseByIdAsync()
		{
			const int warehouseId = 107178;

			var warehouse = await this._globalService.GetWarehouseByIdAsync( warehouseId, CancellationToken.None, Mark.Blank() );

			warehouse.Name.Should().Be( "SkuVault" );
		}
	}
}