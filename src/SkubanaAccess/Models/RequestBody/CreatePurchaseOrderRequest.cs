using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using SkubanaAccess.Shared;

namespace SkubanaAccess.Models.RequestBody
{
	public class CreatePurchaseOrderRequestBody : PurchaseOrderBase
	{
		[ JsonProperty( "paymentTermId" ) ]
		public long PaymentTermId { get; set; }

		[ JsonProperty( "purchaseOrderItems" ) ]
		public IEnumerable< CreatePurchaseOrderItem > Items { get; set; }

		[ JsonProperty( "authorizerUserId" ) ]
		public long? AuthorizerUserId { get; set; }
	}

	public class CreatePurchaseOrderItem
	{
		[ JsonProperty( "originalUnitCost" ) ]
		public Money OriginalUnitCost { get; set; }

		[ JsonProperty( "quantity" ) ]
		public int Quantity { get; set; }

		[ JsonProperty( "unitOfMeasure" ) ] 
		public string UnitOfMeasure { get; set; }

		[ JsonProperty( "uomUnitQuantity" ) ] 
		public int UomUnitQuantity { get; set; }

		[ JsonProperty( "vendorProductId" ) ] 
		public long VendorProductId { get; set; }

		[ JsonProperty( "memo" ) ]
		public string Memo { get; set; }

		[ JsonProperty( "otherCosts" ) ]
		public IEnumerable< OtherCost > OtherCosts { get; set; }


		[ JsonProperty( "discount" ) ]
		public object Discount { get; set; }
		
		[ JsonProperty( "estimatedDeliveryDate" ) ]
		public string EstimatedDeliveryDate { get; set; }

		[ JsonProperty( "holds" ) ]
		public object Holds { get; set; }

		[ JsonProperty( "packaging" ) ]
		public string Packaging { get; set; }

		[ JsonProperty( "percentageTax" ) ]
		public decimal PercentageTax { get; set; }
	}

	public static class CreatePurchaseOrderRequestExtensions
	{
		public static CreatePurchaseOrderRequestBody ToCreatePurchaseOrderRequestBody( this SkubanaPurchaseOrder purchaseOrder, IDictionary< string, long > vendorProductIdsBySku )
		{
			Condition.Requires( purchaseOrder.Vendor, "purchaseOrder.Vendor" ).IsNotNull();
			Condition.Requires( purchaseOrder.Vendor.PaymentTermId, "purchaseOrder.Vendor.PaymentTermId" ).IsNotNull();
			Condition.Requires( purchaseOrder.VendorConfirmByDate, "purchaseOrder.VendorConfirmByDate" ).IsNotNull();
			Condition.Requires( vendorProductIdsBySku, "vendorProductIdsBySku" ).IsNotEmpty();

			return new CreatePurchaseOrderRequestBody
			{
				DestinationWarehouseId = purchaseOrder.DestinationWarehouseId,
				Format = "PO_PDF_ATTACHMENT",		//TODO GUARD-709 Pending question
				PaymentTermId = purchaseOrder.Vendor.PaymentTermId.Value,
				Items = purchaseOrder.Items.Select( x => x.ToCreatePurchaseOrderItem( vendorProductIdsBySku[ x.MasterSku ] ) ),
				Status = purchaseOrder.Status.ToString(),
				VendorConfirmByDate = purchaseOrder.VendorConfirmByDate.Value.ConvertDateTimeToStr(),
				VendorId = purchaseOrder.VendorId,
				AuthorizerUserId = purchaseOrder.AuthorizerUserId,
				AutoUpdatesEnabled = false,
				Currency = purchaseOrder.Currency,
				CustomPurchaseOrderNumber = purchaseOrder.CustomPurchaseOrderNumber,
				IncotermShippingRuleId = purchaseOrder.Vendor.DefaultIncotermShippingRuleId,
				InternalNotes = purchaseOrder.InternalNotes,
				OtherCosts = purchaseOrder.OtherCosts,
				PurchaseOrderTemplateId = purchaseOrder.Vendor.DefaultPurchaseOrderTemplateId
			};
		}

		public static CreatePurchaseOrderItem ToCreatePurchaseOrderItem( this SkubanaPurchaseOrderItem poItem,
			long vendorProductId )
		{
			return new CreatePurchaseOrderItem
			{
				OriginalUnitCost = poItem.OriginalUnitCost,
				Quantity = poItem.Quantity,
				UnitOfMeasure = "Each",
				UomUnitQuantity = poItem.Quantity,
				VendorProductId = vendorProductId,
				Memo = poItem.Memo
			};
		}
	}
}
