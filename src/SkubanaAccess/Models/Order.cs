using Netco.Extensions;
using Newtonsoft.Json;
using SkubanaAccess.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SkubanaAccess.Models
{
	public class Order
	{
		[ JsonProperty( "orderId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "orderNumber" ) ]
		public string Number { get; set; }
		[ JsonProperty( "orderDate" ) ]
		public string OrderDate { get; set; }
		[ JsonProperty( "createdDate" ) ]
		public string CreatedDate { get; set; }
		[ JsonProperty( "modifiedDate" ) ]
		public string ModifiedDate { get; set; }
		[ JsonProperty( "orderStatus" ) ]
		public string Status { get; set; }
		[ JsonProperty( "paymentDate" ) ]
		public string PaymentDate { get; set; }
		[ JsonProperty( "shipCompany" ) ]
		public string ShipCompany { get; set; }
		[ JsonProperty( "shipEmail" ) ]
		public string ShipEmail { get; set; }
		[ JsonProperty( "shipMethod" ) ]
		public ShippingMethod ShippingMethod { get; set; }
		[ JsonProperty( "shipName" ) ]
		public string ShipName { get; set; }
		[ JsonProperty( "shippingCost" ) ]
		public Money ShippingCost { get; set; }
		[ JsonProperty( "shipAddress1" ) ]
		public string ShipAddress1 { get; set; }
		[ JsonProperty( "shipAddress2" ) ]
		public string ShipAddress2 { get; set; }
		[ JsonProperty( "shipAddress3" ) ]
		public string ShipAddress3 { get; set; }
		[ JsonProperty( "shipCity" ) ]
		public string ShipCity { get; set; }
		[ JsonProperty( "shipState" ) ]
		public string ShipState { get; set; }
		[ JsonProperty( "shipZipCode" ) ]
		public string ShipZipCode { get; set; }
		[ JsonProperty( "shipCountry" ) ]
		public string ShipCountry { get; set; }
		[ JsonProperty( "shipPhone" ) ]
		public string ShipPhone { get; set; }
		[ JsonProperty( "orderType" ) ]
		public string Type { get; set; }
		[ JsonProperty( "orderItems" ) ]
		public IEnumerable< OrderItem > Items { get; set; }
		[ JsonProperty( "orderTotal" ) ]
		public Money Total { get; set; }
		[ JsonProperty( "discount" ) ]
		public Money Discount { get; set; }
		[ JsonProperty( "internalNotes" ) ]
		public string Notes { get; set; }
		[ JsonProperty( "currency" ) ]
		public string Currency { get; set; }
		[JsonProperty("shipment")]
		public Shipment Shipment { get; set; }
		[JsonProperty("shipments")]
		public IEnumerable< Shipments > Shipments { get; set; }
	}

	public class OrderItem
	{
		[ JsonProperty( "orderItemId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "listingName" ) ]
		public string ListingName { get; set; }
		[ JsonProperty( "listingSku" ) ]
		public string ListingSku { get; set; }
		[ JsonProperty( "product" ) ]
		public Product Product { get; set; }
		[ JsonProperty( "quantityOrdered" ) ]
		public int QuantityOrdered { get; set; }
		[ JsonProperty( "tax" ) ]
		public Money Tax { get; set; }
		[ JsonProperty( "unitPrice" ) ]
		public Money UnitPrice { get; set; }
		[ JsonProperty( "discount" ) ]
		public Money Discount { get; set; }
	}

	public class Shipment
	{
		[JsonProperty("shipmentId")]
		public long ShipmentId { get; set; }
		[JsonProperty("type")]
		public string ShipmentType { get; set; }
		[JsonProperty("created")]
		public string CreatedDate { get; set; }
		[JsonProperty("shipDate")]
		public string EstimatedShipDate { get; set; }
		[JsonProperty("estimatedArrival")]
		public string EstimatedDeliveryDate { get; set; }
		[JsonProperty("trackingNumber")]
		public string TrackingNumber { get; set; }
		[JsonProperty("deliveryStatus")]
		public string DeliveryStatus { get; set; }
	}

	public class Shipments
	{
		[JsonProperty("items")]
		public IEnumerable< ShipmentsItem > Items { get; set; }
	}

	public class ShipmentsItem
	{
		[JsonProperty("listingSku")]		
		public string Sku { get; set; }
		[JsonProperty("quantity")]
		public int? Quantity { get; set; }
	}

	public class ShippingMethod
	{
		[ JsonProperty( "shippingCarrier" ) ]
		public string Carrier { get; set; }
	}

	public class SkubanaOrder
	{
		public long Id { get; set; }
		public string Number { get; set; }
		public DateTime OrderDateUtc { get; set; }
		public SkubanaOrderStatusEnum Status { get; set; }
		public DateTime PaymentDateUtc { get; set; }
		public IEnumerable< SkubanaOrderItem > Items { get; set; }
		public SkubanaShippingInfo ShippingInfo { get; set; }
		public decimal Total { get; set; }
		public decimal Discount { get; set; }
		public string Notes { get; set; }
	}

	public enum SkubanaOrderStatusEnum 
	{
		Unresolved,
		Awaiting_Payment,
		Awaiting_Shipment,
		Awaiting_MC_Fulfillment,
		Awaiting_3PL_Export,
		Awaiting_Dropship,
		Pending_Fulfillment,
		Shipped,
		Cancelled,
		On_Hold,
		Sku_Unprocessed,
		Processed_Outside_Of_Skubana,
		FBA_Inbound_Shipment_Plan
	}

	public enum SkubanaShipmentTypeEnum
	{
		Fulfillment,
		Return
	}

	public enum SkubanaShipmentDeliveryStatusEnum
	{
		Unknown,
		In_Transit,
		Delivered,
		Lost_In_Transit,
		Return_To_Sender,
		Undeliverable,
		Forwarded,
		Voided		
	}

	public class SkubanaOrderItem
	{
		public string Sku { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Tax { get; set; }
		public decimal Discount { get; set; }
	}

	public class SkubanaShippingInfo
	{
		public SkubanaShippingContactInfo ContactInfo { get; set; }
		public SkubanaShippingAddress Address { get; set; }
		public string Carrier { get; set; }
		public decimal ShippingCost { get; set; }
		public SkubanaShipment Shipment { get; set; }
	}

	public class SkubanaShipment
	{		
		public long ShipmentId { get; set; }	
		public SkubanaShipmentTypeEnum ShipmentType { get; set; }		
		public DateTime CreatedDate { get; set; }		
		public DateTime? EstimatedShipDate { get; set; }		
		public DateTime? EstimatedDeliveryDate { get; set; }		
		public string TrackingNumber { get; set; }		
		public SkubanaShipmentDeliveryStatusEnum DeliveryStatus { get; set; }
		public IEnumerable< SkubanaShipmentItem > Items { get; set; }
	}

	public class SkubanaShipmentItem
	{	
		public string Sku { get; set; }	
		public int Quantity { get; set; }
	}

	public class SkubanaShippingAddress
	{
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Line3 { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
	}

	public class SkubanaShippingContactInfo
	{
		public string Name { get; set; }
		public string CompanyName { get; set; }
		public string PhoneNumber { get; set; }
		public string EmailAddress { get; set; }
	}

	public static class OrdersExtensions
	{
		public static SkubanaOrder ToSVOrder( this Order order )
		{
			var svOrder = new SkubanaOrder()
			{
				Id = order.Id,
				Number = order.Number,
				OrderDateUtc = order.OrderDate.ConvertStrToDateTime().ToUniversalTime(),
				Status = order.Status.ToEnum< SkubanaOrderStatusEnum >( SkubanaOrderStatusEnum.Pending_Fulfillment ),
				PaymentDateUtc = order.PaymentDate.ConvertStrToDateTime().ToUniversalTime(),
				Items = GetOrderItems( order ),
				Total = order.Total?.Amount ?? 0,
				Discount = order.Discount?.Amount ?? 0,
				ShippingInfo = new SkubanaShippingInfo
				{
					Address = new SkubanaShippingAddress
					{
						Line1 = order.ShipAddress1,
						Line2 = order.ShipAddress2,
						Line3 = order.ShipAddress3,
						City = order.ShipCity,
						Country = order.ShipCountry,
						State = order.ShipState,
						PostalCode = order.ShipZipCode
					},
					Carrier = order.ShippingMethod?.Carrier,
					ShippingCost = order.ShippingCost?.Amount ?? 0,
					ContactInfo = new SkubanaShippingContactInfo
					{
						CompanyName = order.ShipCompany,
						Name = order.ShipName,
						EmailAddress = order.ShipEmail,
						PhoneNumber = order.ShipPhone
					}					
				},
				Notes = order.Notes
			};

			if ( order.Shipment != null )
			{ 
				svOrder.ShippingInfo.Shipment = new SkubanaShipment
				{
					ShipmentId = order.Shipment.ShipmentId,
					ShipmentType = order.Shipment.ShipmentType.ToEnum( SkubanaShipmentTypeEnum.Fulfillment ),
					CreatedDate = order.Shipment.CreatedDate.ConvertStrToDateTime().ToUniversalTime(),
					EstimatedShipDate = order.Shipment.EstimatedShipDate?.ConvertStrToDateTime().ToUniversalTime(),
					EstimatedDeliveryDate = order.Shipment.EstimatedDeliveryDate?.ConvertStrToDateTime().ToUniversalTime(),
					TrackingNumber = order.Shipment.TrackingNumber,
					DeliveryStatus = order.Shipment.DeliveryStatus.ToEnum< SkubanaShipmentDeliveryStatusEnum >( SkubanaShipmentDeliveryStatusEnum.Unknown ),
					Items = GetShipmentItems( order.Shipments )
				};
			}

			return svOrder;
		}

		private static IEnumerable< SkubanaOrderItem > GetOrderItems( Order order )
		{
			if ( order.Items == null || !order.Items.Any() )
			{
				return Array.Empty< SkubanaOrderItem >();
			}

			var result = new List< SkubanaOrderItem >();

			foreach( var orderItem in order.Items )
			{
				if ( orderItem.Product == null )
					continue;

				result.Add( new SkubanaOrderItem()
				{
					Sku = orderItem.Product.Sku,
					Quantity = orderItem.QuantityOrdered,
					Discount = orderItem.Discount?.Amount ?? 0,
					UnitPrice = orderItem.UnitPrice?.Amount ?? 0,
					Tax = orderItem.Tax?.Amount ?? 0
				} );
			}

			return result;
		}

		private static IEnumerable< SkubanaShipmentItem > GetShipmentItems( IEnumerable< Shipments > shipments )
		{
			var result = new List< SkubanaShipmentItem >();

			if ( shipments == null || !shipments.Any() )
			{
				return result;
			}			

			foreach( var shipmentItem in shipments.SelectMany( s => s.Items ).ToList() )
			{
				if ( string.IsNullOrWhiteSpace( shipmentItem.Sku ) )
					continue;

				result.Add( new SkubanaShipmentItem
				{
					Sku = shipmentItem.Sku,
					Quantity = shipmentItem?.Quantity ?? 0
				} );
			}

			return result;
		}
	}
}
