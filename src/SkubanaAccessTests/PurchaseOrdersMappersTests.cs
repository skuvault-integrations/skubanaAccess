using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Models;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class PurchaseOrdersMappersTests
	{
		[ Test ]
		public void ToSVPurchaseOrder()
		{
			const long id = 3123;
			const long authorizerUserId = 1234;
			const string currency = "VVV";
			const string internalNotes = "some note";
			const decimal otherCostAmt = 1.2m;
			const string otherCostCurrency = "AAA";
			const string otherCostDescription = "some cost";
			var otherCost = new OtherCost
			{
				Amount = new Money
				{
					Amount = otherCostAmt,
					Currency = otherCostCurrency
				},
				Description = otherCostDescription
			};
			var shippingCost = new Money
			{
				Amount = 1.23m,
				Currency = "MMM"
			};
			const string customPONumber = "1j3k123j2";
			const string dateCreated = "2020-07-02T19:46:10Z";
			var dateCreatedUtc = new DateTime( 2020, 07, 02, 19, 46, 10, DateTimeKind.Utc );
			const string dateModified = "2020-07-03T11:26:10Z";;
			var dateModifiedUtc = new DateTime( 2020, 07, 03, 11, 26, 10, DateTimeKind.Utc );
			const long destinationWarehouseId = 31;
			const string poNumber = "23434431";
			const string status = "AWAITING_VENDOR_CONFIRMATION";
			const long vendorId = 89;
			const long purchaseOrderItemId = 474;
			var items = new []
			{
				new PurchaseOrderItem
				{
					PurchaseOrderItemId = purchaseOrderItemId
				}, 
				new PurchaseOrderItem()
			};
			var purchaseOrder = new PurchaseOrder
			{
				Id = id,
				Authorizer = new User { UserId = authorizerUserId },
				Currency = currency,
				InternalNotes = internalNotes,
				OtherCosts = new [] { otherCost },
				ShippingCost = shippingCost,
				CustomPurchaseOrderNumber = customPONumber,
				DateCreated = dateCreated,
				DateModified = dateModified,
				DestinationWarehouseId = destinationWarehouseId,
				Number = poNumber,
				Status = status,
				VendorId = vendorId,
				PurchaseOrderItems = items
			};

			var result = purchaseOrder.ToSVPurchaseOrder();

			result.Id.Should().Be( id );
			result.AuthorizerUserId.Should().Be( authorizerUserId );
			result.Currency.Should().Be( currency );
			result.InternalNotes.Should().Be( internalNotes );
			var otherCostResult = result.OtherCosts.First();
			otherCostResult.Amount.Amount.Should().Be( otherCost.Amount.Amount );
			otherCostResult.Amount.Currency.Should().Be( otherCost.Amount.Currency );
			otherCostResult.Description.Should().Be( otherCost.Description );
			result.ShippingCost.Should().Be( shippingCost );
			result.CustomPurchaseOrderNumber.Should().Be( customPONumber );
			result.DateCreatedUtc.Should().Be( dateCreatedUtc );
			result.DateModifiedUtc.Should().Be( dateModifiedUtc );
			result.DestinationWarehouseId.Should().Be( destinationWarehouseId );
			result.Number.Should().Be( poNumber );
			result.Status.Should().Be( SkubanaPOStatusEnum.AWAITING_VENDOR_CONFIRMATION );
			result.VendorId.Should().Be( vendorId );
			result.Items.First().ItemId.Should().Be( purchaseOrderItemId );
			result.Items.Count().Should().Be( items.Length );
		}

		[ Test ]
		public void ToSVPurchaseOrderItem()
		{
			const string vendorSku = "testSku43";
			const string memo = "memo1";
			const int quantity = 9;
			const long itemId = 323;
			var billedUnitCost = new Money
			{
				Currency = "HHH",
				Amount = 2.45m
			};
			var originalUnitCost = new Money
			{
				Currency = "NNN",
				Amount = 8.99m
			};
			const string referenceNumber = "123123";
			var purchaseOrderItem = new PurchaseOrderItem
			{
				VendorProductVendorSku = vendorSku,
				Memo = memo,
				Quantity = quantity,
				PurchaseOrderItemId = itemId,
				BilledUnitCost = billedUnitCost,
				OriginalUnitCost = originalUnitCost,
				ReferenceNumber = referenceNumber
			};

			var result = purchaseOrderItem.ToSVPurchaseOrderItem();

			result.VendorSku.Should().Be( vendorSku );
			result.Memo.Should().Be( memo );
			result.Quantity.Should().Be( quantity );
			result.ItemId.Should().Be( itemId );
			result.BilledUnitCost.Should().Be( billedUnitCost );
			result.OriginalUnitCost.Should().Be( originalUnitCost );
			result.ReferenceNumber.Should().Be( referenceNumber );
		}

		[ Test ]
		public void FilterByWarehouse()
		{
			const long warehouseId = 1;
			const long anotherWarehouseId = 2;
			var purchaseOrders = new List< PurchaseOrder >
			{
				new PurchaseOrder
				{
					DestinationWarehouseId = warehouseId
				},
				new PurchaseOrder
				{
					DestinationWarehouseId = anotherWarehouseId
				}
			};

			var result = purchaseOrders.FilterByWarehouse( warehouseId ).ToList();

			result.Count.Should().Be( 1 );
			result.First().DestinationWarehouseId.Should().Be( warehouseId );
		}
	}
}
