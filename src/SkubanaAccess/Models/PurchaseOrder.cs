using System;
using System.Collections.Generic;
using System.Linq;
using Netco.Extensions;
using Newtonsoft.Json;
using SkubanaAccess.Shared; 

namespace SkubanaAccess.Models
{
	public class PurchaseOrder : PurchaseOrderBase
	{
		[ JsonProperty( "purchaseOrderId" ) ]
		public long Id { get; set; }

		[ JsonProperty( "authorizer" ) ]
		public User Authorizer { get; set; }

		[ JsonProperty( "createdBy" ) ]
		public User CreatedBy { get; set; }

		[ JsonProperty( "dateCreated" ) ]
		public string DateCreated { get; set; }

		[ JsonProperty( "dateModified" ) ]
		public string DateModified { get; set; }

		[ JsonProperty( "number" ) ]
		public string Number { get; set; }

		[ JsonProperty( "purchaseOrderItems" ) ]
		public IEnumerable< PurchaseOrderItem > PurchaseOrderItems { get; set; }

		[ JsonProperty( "incotermShippingRule" ) ]
		public string IncotermShippingRule { get; set; }

		[ JsonProperty( "paymentTerm" ) ] 
		public VendorPaymentTerm VendorPaymentTerm { get; set; }

		[ JsonProperty( "dropshipOrderId" ) ]
		public long DropshipOrderId { get; set; }
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
		public IEnumerable< OtherCost > OtherCosts { get; set; }
		public Money ShippingCost { get; set; }
		public SkubanaPOStatusEnum Status { get; set; }
		public long VendorId { get; set; }
		public string PaymentTerm { get; set; }
		public long? AuthorizerUserId { get; set; }
		public DateTime? VendorConfirmByDate { get; set; }
		public SkubanaVendor Vendor { get; set; }
	}

	public class SkubanaPurchaseOrderItem
	{
		public string MasterSku { get; set; }
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
				OtherCosts = purchaseOrder.OtherCosts,
				ShippingCost = purchaseOrder.ShippingCost,
				Status = purchaseOrder.Status.ToEnum< SkubanaPOStatusEnum >( SkubanaPOStatusEnum.AWAITING_AUTHORIZATION ),
				VendorId = purchaseOrder.VendorId,
				PaymentTerm = purchaseOrder.VendorPaymentTerm?.Description ?? ""
			};
		}

		public static SkubanaPurchaseOrderItem ToSVPurchaseOrderItem( this PurchaseOrderItem item )
		{
			return new SkubanaPurchaseOrderItem
			{
				ItemId = item.PurchaseOrderItemId,
				MasterSku = item.VendorProductMasterSku,
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
		AWAITING_AUTHORIZATION,
		AWAITING_VENDOR_CONFIRMATION,
		AWAITING_MODIFICATION_ACCEPTANCE,
		PENDING_DELIVERY,
		PARTIALLY_DELIVERED,
		FULFILLED,CLOSED_SHORT,
		VOIDED,
		CANCELED
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
