using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Models;
using System;
using System.Linq;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class OrderMapperTests
	{
		[ Test ]
		public void ToSVOrder()
		{
			var order = new Order()
			{
				Id = 122323,
				Number = "131301ABC",
				OrderDate = "2020-05-20T12:00:00Z",
				Status = "Awaiting_Shipment",
				PaymentDate = "2020-05-21T19:00:00Z",
				Total = new Money() { Amount = 120.15M, Currency = "USD" },
				Discount = new Money() { Amount = 10.3M, Currency = "USD" },
				Notes = "Internal note",
				ShipName = "Peter Jason Quill",
				ShipCompany = "SkuVault",
				ShipAddress1 = "Pulkovkoe sh.",
				ShipAddress2 = "Moskovskiy district",
				ShipCity = "Saint-Petersburg",
				ShipCountry = "USA",
				ShipPhone = "82349293232",
				ShipEmail = "integrations22322@skuvault.com",
				ShipState = "Saint-Petersburg",
				ShipZipCode = "12345",
				ShippingCost = new Money() { Amount = 9.97M, Currency = "USD" },
				ShippingMethod = new ShippingMethod() { Carrier = "USPS" },
				Items = new OrderItem[]
				{
					new OrderItem()
					{
						Id = 1,
						Product = new Product()
						{
							Sku = "SB-testsku1"
						},
						QuantityOrdered = 10,
						UnitPrice = new Money() { Amount = 1.5M, Currency = "USD" },
						Discount = new Money() { Amount = 0.25M, Currency = "USD" },
						Tax = new Money() { Amount = 0.15M, Currency = "USD" }
					},
					new OrderItem()
					{
						Id = 1,
						Product = new Product()
						{
							Sku = "SB-testsku2"
						},
						QuantityOrdered = 15,
						UnitPrice = new Money() { Amount = 2.2M, Currency = "USD" },
						Discount = new Money() { Amount = 0, Currency = "USD" },
						Tax = new Money() { Amount = 0.5M, Currency = "USD" }
					},
				}
			};

			var svOrder = order.ToSVOrder();

			svOrder.Id.Should().Be( order.Id );
			svOrder.Number.Should().Be( order.Number );
			svOrder.Status.Should().Be( SkubanaOrderStatusEnum.Awaiting_Shipment );
			svOrder.OrderDateUtc.Should().Be( new DateTime( 2020, 05, 20, 12, 0, 0 ) );
			svOrder.PaymentDateUtc.Should().Be( new DateTime( 2020, 05, 21, 19, 0, 0 ) );
			svOrder.Total.Should().Be( order.Total.Amount );
			svOrder.Discount.Should().Be( order.Discount.Amount );
			svOrder.Notes.Should().Be( order.Notes );

			svOrder.ShippingInfo.ShippingCost.Should().Be( order.ShippingCost.Amount );
			svOrder.ShippingInfo.Carrier.Should().Be( order.ShippingMethod.Carrier );
			
			svOrder.ShippingInfo.Address.Line1.Should().Be( order.ShipAddress1 );
			svOrder.ShippingInfo.Address.Line2.Should().Be( order.ShipAddress2 );
			svOrder.ShippingInfo.Address.PostalCode.Should().Be( order.ShipZipCode );
			svOrder.ShippingInfo.Address.State.Should().Be( order.ShipState );
			svOrder.ShippingInfo.Address.City.Should().Be( order.ShipCity );
			svOrder.ShippingInfo.Address.Country.Should().Be( order.ShipCountry );

			svOrder.ShippingInfo.ContactInfo.Name.Should().Be( order.ShipName );
			svOrder.ShippingInfo.ContactInfo.PhoneNumber.Should().Be( order.ShipPhone );
			svOrder.ShippingInfo.ContactInfo.EmailAddress.Should().Be( order.ShipEmail );
			svOrder.ShippingInfo.ContactInfo.CompanyName.Should().Be( order.ShipCompany );

			svOrder.Items.Count().Should().Be( order.Items.Count() );
			svOrder.Items.First().Sku.Should().Be( order.Items.First().Product.Sku );
			svOrder.Items.First().Quantity.Should().Be( order.Items.First().QuantityOrdered );
			svOrder.Items.First().Tax.Should().Be( order.Items.First().Tax.Amount );
			svOrder.Items.First().Discount.Should().Be( order.Items.First().Discount.Amount );
		}

		[ Test ]
		public void ToSVOrderWithUnknownStatus()
		{
			var order = new Order()
			{
				Status = "New order status"
			};

			var svOrder = order.ToSVOrder();

			svOrder.Status.Should().Be( SkubanaOrderStatusEnum.Pending_Fulfillment );
		}
	}
}
