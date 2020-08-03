using System;
using System.Collections.Generic;
using System.Linq;
using Netco.Extensions;
using Newtonsoft.Json;
using SkubanaAccess.Shared; 

namespace SkubanaAccess.Models
{
	public class PurchaseOrder
	{
		[ JsonProperty( "purchaseOrderId" ) ]
		public long Id { get; set; }

		[ JsonProperty( "authorizer" ) ]
		public User Authorizer { get; set; }

		[ JsonProperty( "createdBy" ) ]
		public User CreatedBy { get; set; }

		[ JsonProperty( "currency") ]
		public string Currency { get; set; }

		[ JsonProperty( "dateAuthorized" ) ]
		public string DateAuthorized { get; set; }

		[ JsonProperty( "dateCreated" ) ]
		public string DateCreated { get; set; }

		[ JsonProperty( "dateModified" ) ]
		public string DateModified { get; set; }

		[ JsonProperty( "dateIssued" ) ]
		public string DateIssued { get; set; }

		[ JsonProperty( "destinationWarehouseId" ) ]
		public long DestinationWarehouseId { get; set; }

		[ JsonProperty( "format" ) ]
		public string Format { get; set; }

		[ JsonProperty( "internalNotes" ) ]
		public string InternalNotes { get; set; }

		[ JsonProperty( "number" ) ]
		public string Number { get; set; }

		[ JsonProperty( "customPurchaseOrderNumber" ) ]
		public string CustomPurchaseOrderNumber { get; set; }

		[ JsonProperty( "purchaseOrderItems" ) ]
		public IEnumerable< PurchaseOrderItem > PurchaseOrderItems { get; set; }

		[ JsonProperty( "otherCosts" ) ]
		public IEnumerable< OtherCost > OtherCosts { get; set; }

		[ JsonProperty( "assignedPurchaseOrderMilestones" ) ]
		public object AssignedPurchaseOrderMilestones { get; set; }

		[ JsonProperty( "messagesToVendor" ) ]
		public object MessagesToVendor { get; set; }	//Not using because api always returns as empty

		[ JsonProperty( "shippingCost" ) ] 
		public Money ShippingCost { get; set; }

		[ JsonProperty( "status" ) ]
		public string Status { get; set; }

		[ JsonProperty( "type" ) ]
		public string Type { get; set; }

		[ JsonProperty( "vendorConfirmByDate" ) ]
		public string VendorConfirmByDate { get; set; }

		[ JsonProperty( "vendorConfirmedOnDate" ) ]
		public string VendorConfirmedOnDate { get; set; }

		[ JsonProperty( "vendorId" ) ]
		public long VendorId { get; set; }

		[ JsonProperty( "incotermShippingRule" ) ]
		public string IncotermShippingRule { get; set; }

		[ JsonProperty( "incotermShippingRuleId" ) ]
		public long IncotermShippingRuleId { get; set; }

		[ JsonProperty( "paymentTerm" ) ] 
		public PaymentTerm PaymentTerm { get; set; }

		[ JsonProperty( "dropshipOrderId" ) ]
		public long DropshipOrderId { get; set; }

		[ JsonProperty( "purchaseOrderTemplateId" ) ]
		public long PurchaseOrderTemplateId { get; set; }

		[ JsonProperty( "autoUpdatesEnabled" ) ]
		public bool AutoUpdatesEnabled { get; set; }
	}

	public class User
	{
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }

		[ JsonProperty( "email" ) ]
		public string Email { get; set; }

		[ JsonProperty( "userId" ) ]
		public long UserId { get; set; }
	}

	public class PaymentTerm
	{
		[ JsonProperty( "vendorPaymentTermId" ) ]
		public long VendorPaymentTermId { get; set; }

		[ JsonProperty( "description" ) ]
		public string Description { get; set; }

		[ JsonProperty( "active" ) ]
		public bool Active { get; set; }
	}

	public class OtherCost
	{
		[ JsonProperty( "description" ) ] 
		public string Description { get; set; }

		[ JsonProperty( "amount" ) ] 
		public Money Amount { get; set; }
	}

	public class PurchaseOrderItem
	{
		[ JsonProperty( "purchaseOrderItemId" ) ] 
		public long PurchaseOrderItemId { get; set; }

		[ JsonProperty( "billedDate" ) ] 
		public string BilledDate { get; set; }

		[ JsonProperty( "billedUnitCost" ) ] 
		public Money BilledUnitCost { get; set; }

		[ JsonProperty( "deliveryDate" ) ] 
		public string DeliveryDate { get; set; }

		[ JsonProperty( "estimatedDeliveryDate" ) ] 
		public string EstimatedDeliveryDate { get; set; }

		[ JsonProperty( "landedUnitCost" ) ] 
		public Money LandedUnitCost { get; set; }

		[ JsonProperty( "discount" ) ] 
		public Discount Discount { get; set; }
		
		[ JsonProperty( "memo" ) ] 
		public string Memo { get; set; }

		[ JsonProperty( "originalUnitCost" ) ] 
		public Money OriginalUnitCost { get; set; }

		[ JsonProperty( "packaging" ) ] 
		public string Packaging { get; set; }

		[ JsonProperty( "percentageTax" ) ] 
		public decimal PercentageTax { get; set; }

		[ JsonProperty( "quantity" ) ]
		public int Quantity { get; set; }

		[ JsonProperty( "referenceNumber" ) ] 
		public string ReferenceNumber { get; set; }

		[ JsonProperty( "status" ) ] 
		public string Status { get; set; }

		[ JsonProperty( "unitOfMeasure" ) ] 
		public string UnitOfMeasure { get; set; }

		[ JsonProperty( "uomUnitQuantity" ) ]
		public int? UomUnitQuantity { get; set; }

		[ JsonProperty( "productStockId" ) ]
		public int? ProductStockId { get; set; }

		[ JsonProperty( "holds" ) ]
		public object Holds { get; set; }

		[ JsonProperty( "vendorProductId" ) ]
		public int? VendorProductId { get; set; }

		[ JsonProperty( "vendorProductMasterSku" ) ]
		public string VendorProductMasterSku { get; set; }

		[ JsonProperty( "vendorProductVendorSku" ) ]
		public string VendorProductVendorSku { get; set; }

		[ JsonProperty( "receivedDate" ) ] 
		public string ReceivedDate { get; set; }
	}

	public class Discount
	{
		[ JsonProperty( "amount" ) ] 
		public decimal Amount { get; set; }

		[ JsonProperty( "discountType" ) ] 
		public string DiscountType { get; set; }
	}

	public class SkubanaPurchaseOrder
	{
		public long Id { get; set; }
		public string Currency { get; set; }
		public DateTime DateCreatedUtc { get; set; }
		public DateTime? DateModifiedUtc { get; set; }
		public long DestinationWarehouseId { get; set; }
		public string InternalNotes { get; set; }
		public string Number { get; set; }
		public string CustomPurchaseOrderNumber { get; set; }
		public IEnumerable< SkubanaPurchaseOrderItem > Items { get; set; }
		public IEnumerable< SkubanaOtherCosts > OtherCosts { get; set; }
		public Money ShippingCost { get; set; }
		public SkubanaPOStatusEnum Status { get; set; }
		public long VendorId { get; set; }
		public string VendorName { get; set; }
		public string PaymentTerm { get; set; }
		public long? AuthorizerUserId { get; set; }
	}

	public class SkubanaPurchaseOrderItem
	{
		public string VendorSku { get; set; }
		public long ItemId { get; set; }
		public Money BilledUnitCost { get; set; }
		public string Memo { get; set; }
		public Money OriginalUnitCost { get; set; }
		public int Quantity { get; set; }
		public string ReferenceNumber { get; set; }
	}

	public static class PurchaseOrderExtensions
	{
		public static SkubanaPurchaseOrder ToSVPurchaseOrder( this PurchaseOrder purchaseOrder )
		{
			var items = purchaseOrder.PurchaseOrderItems ?? new List< PurchaseOrderItem >();

			return new SkubanaPurchaseOrder
			{
				Id = purchaseOrder.Id,
				AuthorizerUserId = purchaseOrder.Authorizer?.UserId,
				Currency = purchaseOrder.Currency,
				DateCreatedUtc = purchaseOrder.DateCreated.ConvertStrToDateTime().ToUniversalTime(),
				DateModifiedUtc = purchaseOrder.DateModified?.ConvertStrToDateTime().ToUniversalTime(),
				DestinationWarehouseId = purchaseOrder.DestinationWarehouseId,
				InternalNotes = purchaseOrder.InternalNotes,
				Number = purchaseOrder.Number,
				CustomPurchaseOrderNumber = purchaseOrder.CustomPurchaseOrderNumber,
				Items = items.Select( i => i.ToSVPurchaseOrderItem() ),
				OtherCosts = purchaseOrder.OtherCosts?.Select( o => new SkubanaOtherCosts
				{
					Description = o.Description,
					Amount = o.Amount
				} ),
				ShippingCost = purchaseOrder.ShippingCost,
				Status = purchaseOrder.Status.ToEnum< SkubanaPOStatusEnum >( SkubanaPOStatusEnum.Undefined ),
				VendorId = purchaseOrder.VendorId,
				PaymentTerm = purchaseOrder.PaymentTerm?.Description ?? ""
			};
		}

		public static SkubanaPurchaseOrderItem ToSVPurchaseOrderItem( this PurchaseOrderItem item )
		{
			return new SkubanaPurchaseOrderItem
			{
				ItemId = item.PurchaseOrderItemId,
				VendorSku = item.VendorProductVendorSku,
				BilledUnitCost = item.BilledUnitCost,
				Memo = item.Memo,
				OriginalUnitCost = item.OriginalUnitCost,
				Quantity = item.Quantity,
				ReferenceNumber = item.ReferenceNumber
			};
		}

		public static IEnumerable< PurchaseOrder > FilterByWarehouse( this IEnumerable< PurchaseOrder > purchaseOrders, long warehouseId )
		{
			return purchaseOrders.Where( p => p.DestinationWarehouseId == warehouseId );
		}

	}

	public enum SkubanaPOStatusEnum
	{
		Undefined,
		AWAITING_AUTHORIZATION,
		AWAITING_VENDOR_CONFIRMATION,
		AWAITING_MODIFICATION_ACCEPTANCE,
		PENDING_DELIVERY,
		PARTIALLY_DELIVERED,
		FULFILLED,CLOSED_SHORT,
		VOIDED,
		CANCELED
	}

	public class SkubanaOtherCosts
	{
		public string Description { get; set; }
		public Money Amount { get; set; }
	}

	public class PurchaseOrderComparer : IEqualityComparer< SkubanaPurchaseOrder >
	{
		public bool Equals( SkubanaPurchaseOrder x, SkubanaPurchaseOrder y )
		{
			if( ReferenceEquals( x, null ) )
				return false;
			if( ReferenceEquals( null, y ) )
				return false;
			if( ReferenceEquals( x, y ) )
				return true;
			return x.Id == y.Id;
		}

		public int GetHashCode( SkubanaPurchaseOrder obj )
		{
			return ( int ) obj.Id;
		}
	}
}
