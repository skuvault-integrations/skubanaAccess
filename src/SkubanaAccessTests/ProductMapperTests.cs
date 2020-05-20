using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Models;
using System.Linq;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class ProductMapperTests
	{
		[ Test ]
		public void ToSVProduct()
		{
			var product = new Product()
			{
				Id = 1,
				Type = "CORE_PRODUCT",
				Sku = "SB-testsku1",
				Categories = new string[] { "Clothes", "Apparel" },
				Name = "SB-testsku1",
				Upc = "78942",
				Brand = "Samsung",
				MapPrice = new Money() { Amount = 5.6M, Currency = "USD" },
				VendorCost = new Money() { Amount = 15.5M, Currency = "USD" },
				Weight = 1.2M,
				Mpn = "12345",
				Description = "Here test sku description",
				ImageUrls = new ProductImage[]
				{
					new ProductImage()
					{
						Id = 1,
						Url = "https://domain.com/image1.jpg",
						Rank = 1
					},
					new ProductImage()
					{
						Id = 2,
						Url = "https://domain.com/image2.jpg",
						Rank = 2
					},
				},
				AttributeGroups = new ProductAttributeGroup[]
				{
					new ProductAttributeGroup()
					{
						Id = 1,
						Name = "Color",
						Attributes = new ProductAttribute[]
						{
							new ProductAttribute()
							{
								Id = 1,
								Name = "Gold"
							}
						}
					},
					new ProductAttributeGroup()
					{
						Id = 2,
						Name = "Size",
						Attributes = new ProductAttribute[]
						{
							new ProductAttribute()
							{
								Id = 1,
								Name = "46"
							}
						}
					},
				},
				IsActive = true
			};

			var svProduct = product.ToSVProduct();

			svProduct.Id.Should().Be( product.Id );
			svProduct.Sku.Should().Be( product.Sku );
			svProduct.Upc.Should().Be( product.Upc );
			svProduct.Type.Should().Be( product.Type );
			svProduct.Classifications.Should().BeEquivalentTo( product.Categories );
			svProduct.Name.Should().Be( product.Name );
			svProduct.BrandName.Should().Be( product.Brand );
			svProduct.Price.Should().Be( product.MapPrice.Amount );
			svProduct.Cost.Should().Be( product.VendorCost.Amount );
			svProduct.Weight.Should().Be( product.Weight );
			svProduct.PartNumber.Should().Be( product.Mpn );
			svProduct.LongDescription.Should().Be( product.Description );
			svProduct.ImagesUrls.Should().BeEquivalentTo( product.ImageUrls.Select( i => i.Url ) );
			svProduct.Attributes.Count.Should().Be( product.AttributeGroups.Count() );
			svProduct.Attributes.First().Key.Should().Be( product.AttributeGroups.First().Name );
			svProduct.Attributes.First().Value.Should().Be( product.AttributeGroups.First().Attributes.First().Name );
			svProduct.IsActive.Should().Be( product.IsActive );
		}
	}
}