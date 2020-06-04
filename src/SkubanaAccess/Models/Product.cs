using CuttingEdge.Conditions;
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
		[ JsonProperty( "attributeGroups" ) ]
		public IEnumerable< ProductAttributeGroup > AttributeGroups { get; set; }
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

	public struct ProductType
	{
		public static ProductType CoreProduct = new ProductType( "CORE_PRODUCT" );
		public static ProductType VirtualProduct = new ProductType( "VIRTUAL_PRODUCT" );

		public string Type { get; private set; }

		public ProductType( string type )
		{
			Condition.Requires( type, "type" ).IsNotNullOrEmpty();
			this.Type = type;
		}
	}

	public class ProductAttributeGroup
	{
		[ JsonProperty( "attributeGroupId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "attributes" ) ]
		public IEnumerable< ProductAttribute > Attributes { get; set; }
	}

	public class ProductAttribute
	{
		[ JsonProperty( "attributeId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
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
		public IDictionary< string, string > Attributes { get; set; }
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
				ImagesUrls = GetImageUrls( product.ImageUrls ),
				Attributes = GetAttributes( product.AttributeGroups ),
				IsActive = product.IsActive
			};
		}

		private static IEnumerable< string > GetImageUrls( IEnumerable< ProductImage > images )
		{
			if ( images == null || !images.Any() )
				return Array.Empty< string >();

			return images.Where( i => !string.IsNullOrEmpty( i.Url ) )
						.OrderBy( i => i.Rank )
						.Select( i => i.Url );
		}

		private static IDictionary< string, string > GetAttributes( IEnumerable< ProductAttributeGroup > attributeGroups )
		{
			if ( attributeGroups == null || !attributeGroups.Any() )
				return new Dictionary< string, string >();

			var attributes = new Dictionary< string, string >();
			foreach( var attributeGroup in attributeGroups )
			{
				var attributeName = attributeGroup.Name;

				if ( string.IsNullOrEmpty( attributeName ) )
					continue;

				var attributeValue = attributeGroup.Attributes?.FirstOrDefault( a => !string.IsNullOrEmpty( a.Name ) );

				if ( attributeValue != null )
				{
					if ( !attributes.ContainsKey( attributeName ) )
					{
						attributes.Add( attributeName, attributeValue.Name );
					}
				}
			}

			return attributes;
		}
	}
}