using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Models;
using SkubanaAccess.Models.RequestBody;
using SkubanaAccess.Shared;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class CreatePurchaseOrderRequestMapperTests
	{
		[ Test ]
		public void ToCreatePurchaseOrderRequestBody()
		{
			const string itemSku = "testSku1";
			var poItem = new SkubanaPurchaseOrderItem
			{
				MasterSku = itemSku,
				Quantity = 19
			};
			const string skubanaPoStatus = "PENDING_DELIVERY";
			var vendorConfirmByDate = new DateTime( 2222, 1, 1 );
			const long vendorId = 9494;
			const string currency = "ABC";
			const long authorizerUserId = 9494;
			const string customPurchaseOrderNumber = "custNum123";
			const string internalNotes = "internal note 1";
			var otherCostAmount1 = new Money
			{
				Amount = 1.23m, 
				Currency = "VVV"
			};
			const string otherCostDescription1 = "cost X";
			var otherCosts = new List< OtherCost >
			{
				new OtherCost
				{
					Amount = otherCostAmount1, 
					Description = otherCostDescription1
				}
			};
			var vendor = new SkubanaVendor
			{
				PaymentTermId = 123,
				DefaultIncotermShippingRuleId = 234,
				DefaultPurchaseOrderTemplateId = 987
			};
			var po = new SkubanaPurchaseOrder
			{
				DestinationWarehouseId = 8383,
				Items = new List< SkubanaPurchaseOrderItem > { poItem },
				Status = SkubanaPOStatusEnum.PENDING_DELIVERY,
				VendorConfirmByDate = vendorConfirmByDate,
				VendorId = vendorId,
				Vendor = vendor,
				Currency = currency,
				AuthorizerUserId = authorizerUserId,
				CustomPurchaseOrderNumber = customPurchaseOrderNumber,
				InternalNotes = internalNotes,
				OtherCosts = otherCosts
			};

			const long vendorProductId = 213;
			var vendorProductIds = new Dictionary< string, long >
			{
				{ itemSku, vendorProductId }
			};

			var result = po.ToCreatePurchaseOrderRequestBody( vendorProductIds );

			result.DestinationWarehouseId.Should().Be( po.DestinationWarehouseId );
			result.Format.Should().Be( "PO_PDF_ATTACHMENT" );
			result.PaymentTermId.Should().Be( vendor.PaymentTermId );
			result.VendorConfirmByDate.Should().Be( vendorConfirmByDate.ConvertDateTimeToStr() );
			result.Status.Should().Be( skubanaPoStatus );
			result.VendorId.Should().Be( vendorId );
			result.Currency.Should().Be( currency );
			result.AuthorizerUserId.Should().Be( authorizerUserId );
			result.AutoUpdatesEnabled.Should().Be( false );
			result.CustomPurchaseOrderNumber.Should().Be( customPurchaseOrderNumber );
			result.IncotermShippingRuleId.Should().Be( vendor.DefaultIncotermShippingRuleId );
			result.InternalNotes.Should().Be( internalNotes );
			result.OtherCosts.Should().BeEquivalentTo( otherCosts );
			result.PurchaseOrderTemplateId.Should().Be( vendor.DefaultPurchaseOrderTemplateId );
			result.Items.Count().Should().Be( 1 );
			result.Items.First().VendorProductId.Should().Be( vendorProductId );
			result.Items.First().Quantity.Should().Be( poItem.Quantity );
		}

		[ Test ]
		public void ToCreatePurchaseOrderItem()
		{
			var originalCost = new Money
			{
				Amount = 1.23m,
				Currency = "GPU"
			};
			const int quantity = 12;
			const string memo = "It's a mee, Mariooo";
			var poItem = new SkubanaPurchaseOrderItem
			{
				OriginalUnitCost = originalCost,
				Quantity = quantity,
				Memo = memo
			};
			const long vendorProductId = 123123;

			var result = poItem.ToCreatePurchaseOrderItem( vendorProductId );

			result.OriginalUnitCost.Should().Be( originalCost );
			result.Quantity.Should().Be( quantity );
			result.UnitOfMeasure.Should().Be( "Each" );
			result.UomUnitQuantity.Should().Be( quantity );
			result.VendorProductId.Should().Be( vendorProductId );
			result.Memo.Should().Be( memo );
		}
	}
}
