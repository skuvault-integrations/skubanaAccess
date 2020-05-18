using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkubanaAccess.Models
{
	public class Product
	{
		[ JsonProperty( "productId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "masterSku" ) ]
		public string Sku { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "type" ) ]
		public string Type { get; set; }
		[ JsonProperty( "upc" ) ]
		public string Upc { get; set; }
		[ JsonProperty( "mpn" ) ]
		public string Mpn { get; set; }
		[ JsonProperty( "description" ) ]
		public string Description { get; set; }
		[ JsonProperty( "hazmat" ) ]
		public bool Hazmat { get; set; }
		[ JsonProperty( "active" ) ]
		public bool IsActive { get; set; }
		[ JsonProperty( "brand" ) ]
		public string Brand { get; set; }
		[ JsonProperty( "categories" ) ]
		public string[] Categories { get; set; }
		[ JsonProperty( "createdDate" ) ]
		public DateTime CreatedDate { get; set; }
		[ JsonProperty( "imageUrls" ) ]
		public ProductImage[] ImageUrls { get; set; }
		[ JsonProperty( "modifiedDate" ) ]
		public DateTime ModifiedDate { get; set; }
		[ JsonProperty( "mapPrice" ) ]
		public Money MapPrice { get; set; }
		[ JsonProperty( "vendorCost" ) ]
		public Money VendorCost { get; set; }
		[ JsonProperty( "weight" ) ]
		public decimal Weight { get; set; }
		[ JsonProperty( "width" ) ]
		public decimal Width { get; set; }
		[ JsonProperty( "height" ) ]
		public decimal Height { get; set; }
		[ JsonProperty( "length" ) ]
		public decimal Length { get; set; }
		[ JsonProperty( "digital" ) ]
		public bool IsDigital { get; set; }
	}

	public class Money
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; }
	}

	public class ProductImage
	{
		[ JsonProperty( "id" ) ]
		public long Id { get; set; }
		[ JsonProperty( "url" ) ]
		public string Url { get; set; }
		[ JsonProperty( "rank" ) ]
		public int Rank { get; set; }
	}

	public class SkubanaProduct
	{
		public long Id { get; set; }
		public string Type { get; set; }
		public string Sku { get; set; }
		public IEnumerable< string > Classifications { get; set; }
		public string Name { get; set; }
		public string Upc { get; set; }
		public string BrandName { get; set; }
		public decimal Price { get; set; }
		public decimal Cost { get; set; }
		public decimal Weight { get; set; }
		public string PartNumber { get; set; }
		public string LongDescription { get; set; }
		public IEnumerable< string > ImagesUrls { get; set; }
		public bool IsActive { get; set; }
	}

	public static class ProductsExtensions
	{
		public static SkubanaProduct ToSVProduct( this Product product )
		{
			return new SkubanaProduct()
			{
				Id = product.Id,
				Type = product.Type,
				Sku = product.Sku,
				Upc = product.Upc,
				Classifications = product.Categories,
				Name = product.Name,
				BrandName = product.Brand,
				Price = product.MapPrice?.Amount ?? 0,
				Cost = product.VendorCost?.Amount ?? 0,
				Weight = product.Weight,
				PartNumber = product.Mpn,
				LongDescription = product.Description,
				ImagesUrls = product.ImageUrls.OrderBy( i => i.Rank ).Select( i => i.Url ),
				IsActive = product.IsActive
			};
		}
	}
}