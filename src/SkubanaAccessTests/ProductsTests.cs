using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Services.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class ProductsTests : BaseTest
	{
		private IProductsService _productService;
		private const string _testSku1 = "SB-testsku1";
		private const string _testSku2 = "SB-testsku2";

		[ SetUp ]
		public void Init()
		{
			this._productService = new ProductsService( base.Config );
		}

		[ Test ]
		public async Task GetProductsBySkus()
		{
			var skus = new string[] { _testSku1, _testSku2 };

			var products = await this._productService.GetProductsBySkus( skus, CancellationToken.None );
			products.Count().Should().Be( 2 );
		}

		[ Test ]
		public async Task GetProductThatNotExists()
		{
			var skus = new string[] { "SB-" + Guid.NewGuid().ToString() };
			var products = await this._productService.GetProductsBySkus( skus, CancellationToken.None );
			products.Should().BeNullOrEmpty();
		}

		[ Test ]
		public async Task GetProductsUpdatedAfter()
		{
			var products = await this._productService.GetProductsUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -1 ), CancellationToken.None );
			products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetProductsUpdateAfterUsingSmallPage()
		{
			base.Config.RetrieveProductsPageSize = 1;
			var products = await this._productService.GetProductsUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -1 ), CancellationToken.None );
			products.Should().NotBeNullOrEmpty();
		}
	}
}
