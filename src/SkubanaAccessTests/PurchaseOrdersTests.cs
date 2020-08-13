using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Models;
using SkubanaAccess.Services.Products;
using SkubanaAccess.Services.PurchaseOrders;
using SkubanaAccess.Shared;
using Mark = Netco.Logging.Mark;

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
			var productsService = new ProductsService( base.Config );
			this._purchaseOrdersService = new PurchaseOrdersService( base.Config, productsService );
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

			var results = await _purchaseOrdersService.GetVendorsByIdsAsync( vendorIds, CancellationToken.None, Mark.Blank() );

			results.Count.Should().Be( 1 );
			results[ vendorId ].Name.Should().Be( "Amazon" );
		}

		[ Test ]
		public async Task GetVendorsAsync()
		{
			var vendors = await _purchaseOrdersService.GetVendorsAsync( CancellationToken.None, Mark.Blank() );

			vendors.Should().NotBeEmpty();
		}

		[ Test ]
		public async Task GetVendorProductIdByProductIdVendorIdAsync()
		{
			const long vendorId = 762;
			const long productId = 681732;

			var result = await _purchaseOrdersService.GetVendorProductIdByProductIdVendorIdAsync( productId, vendorId, CancellationToken.None, Mark.Blank() );

			result.Should().Be( 34960 );
		}

		[ Test ]
		public async Task CreatePurchaseOrdersAsync()
		{
			const long warehouseId = 107178;
			const int vendorId = 755;
			var items = new []
			{
				new SkubanaPurchaseOrderItem
				{
					MasterSku = "SB-productpull1",
					OriginalUnitCost = new Money
					{
						Amount = 2.34m,
						Currency = "SCR"
					},
					Quantity = 21,
					Memo = "some memo"
				}
			};
			var customerPONum = DateTime.UtcNow.ToString( "s" );
			var newPurchaseOrders = new []
			{
				new SkubanaPurchaseOrder
				{
					AuthorizerUserId = 1347,
					Currency = "USD",
					Items = items,
					CustomPurchaseOrderNumber = customerPONum,
					Vendor = new SkubanaVendor
					{
						PaymentTermId = 2,
						DefaultIncotermShippingRuleId = 1,
						DefaultPurchaseOrderTemplateId = 10,
						VendorId = vendorId
					},
					DestinationWarehouseId = warehouseId,
					Status = SkubanaPOStatusEnum.AWAITING_AUTHORIZATION,
					InternalNotes = "int notes",
					OtherCosts = new []
					{
						new OtherCost
						{
							Amount = new Money
							{
								Amount = 1.2m,
								Currency = "GYD"
							},
							Description = "stuff"
						}
					},
					DateModifiedUtc = DateTime.UtcNow,
					ShippingCost = new Money
					{
						Amount = 2.3m,
						Currency = "BAM"
					},
					DateCreatedUtc = DateTime.UtcNow,
					VendorConfirmByDate = DateTime.UtcNow.AddMonths( 3 )
				}
			};

			await this._purchaseOrdersService.CreatePurchaseOrdersAsync( newPurchaseOrders, CancellationToken.None, Mark.Blank() );

			var purchaseOrders = await this._purchaseOrdersService.GetPOsModifiedCreatedInDateRangeAsync( DateTime.UtcNow.AddMinutes( -2 ), DateTime.UtcNow, warehouseId, CancellationToken.None, Mark.Blank() );

			var purchaseOrder = purchaseOrders.FirstOrDefault( x => x.CustomPurchaseOrderNumber == customerPONum );
			purchaseOrder.Should().NotBeNull();
		}
	}
}
