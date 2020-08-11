using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Services.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class InventoryTests : BaseTest
	{
		private IInventoryService _inventoryService;
		private const long _warehouseId = 107178;
		private const long _inHouseWarehouseId = 107175; 
		private const string _testSku1 = "SB-testsku1";
		private const string _testSku2 = "SB-testsku2";
		private const string _testSku3 = "SB-testsku3";
		
		private const long _testProductWithoutStock = 681563;
		private const string _testSkuWithoutStock = "SB-testsku8";

		[ SetUp ]
		public void Init()
		{
			this._inventoryService = new InventoryService( base.Config );
		}

		[ Test ]
		public async Task GetSkuQuantity()
		{
			var stock = await this._inventoryService.GetProductStock( _testSku1, _warehouseId, CancellationToken.None );
			stock.Should().NotBeNull();
			stock.OnHandQuantity.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		[ Ignore( "Make sure that product's stock doesn't exist in Skubana" ) ]
		public async Task GetSkuQuantityWhenStockDoesntExist()
		{
			var stock = await this._inventoryService.GetProductStock( _testSkuWithoutStock, _warehouseId, CancellationToken.None );
			stock.Should().BeNull();
		}

		[ Test ]
		public async Task UpdateSkuQuantity()
		{
			Thread.Sleep( 10 * 1000 );
			int newQuantity = new Random().Next( 1, 100 );

			await this._inventoryService.AdjustProductStockQuantityTo3PLWarehouse( _testSku1, newQuantity, _warehouseId, CancellationToken.None );
			var stock = await this._inventoryService.GetProductStock( _testSku1, _warehouseId, CancellationToken.None );

			stock.OnHandQuantity.Should().Be( newQuantity );
		}

		[ Test ]
		public async Task UpdateSkuQuantityToZero()
		{
			Thread.Sleep( 10 * 1000 );
			await this._inventoryService.AdjustProductStockQuantityTo3PLWarehouse( _testSku1, 0, _warehouseId, CancellationToken.None );
			var stock = await this._inventoryService.GetProductStock( _testSku1, _warehouseId, CancellationToken.None );

			stock.OnHandQuantity.Should().Be( 0 );
		}

		[ Test ]
		public async Task UpdateSkuQuantityToQuantityThatExceedMaxQuantity()
		{
			Thread.Sleep( 10 * 1000 );
			int maxQuantity = 100 * 1000 * 1000 + 1; // 100,000,000 + 1
			await this._inventoryService.AdjustProductStockQuantityTo3PLWarehouse( _testSku1, maxQuantity, _warehouseId, CancellationToken.None );
			var stock = await this._inventoryService.GetProductStock( _testSku1, _warehouseId, CancellationToken.None );

			stock.OnHandQuantity.Should().Be( maxQuantity - 1 );
		}

		[ Test ]
		public async Task UpdateSkusQuantities()
		{
			Thread.Sleep( 10 * 1000 );
			var random = new Random();
			var skusQuantities = new Dictionary< string, int >()
			{
				{ _testSku1, random.Next( 1, 100 ) },
				{ _testSku3, random.Next( 1, 100 ) }
			};

			await this._inventoryService.AdjustProductsStockQuantitiesTo3PLWarehouse( skusQuantities, _warehouseId, CancellationToken.None );
			
			foreach( var skuQuantity in skusQuantities )
			{
				var stock = await this._inventoryService.GetProductStock( skuQuantity.Key, _warehouseId, CancellationToken.None );
				stock.OnHandQuantity.Should().Be( skuQuantity.Value );
			}
		}

		[ Test ]
		public async Task UpdateSkusQuantitiesWhenOneOfTheSkusDoesntHaveStock()
		{
			Thread.Sleep( 10 * 1000 );
			var random = new Random();
			var skusQuantities = new Dictionary< string, int >()
			{
				{ _testSku1, random.Next( 1, 100 ) },
				{ _testSkuWithoutStock, random.Next( 1, 100 ) }
			};

			var response = await this._inventoryService.AdjustProductsStockQuantitiesTo3PLWarehouse( skusQuantities, _warehouseId, CancellationToken.None );
			response.ProductsWithoutStocks.Count.Should().Be( 1 );
		}

		[ Test ]
		public async Task UpdateSkusQuantitiesWhenOneOfTheSkusDoesntExist()
		{
			Thread.Sleep( 10 * 1000 );
			var random = new Random();
			var skusQuantities = new Dictionary< string, int >()
			{
				{ _testSku1, random.Next( 1, 100 ) },
				{ Guid.NewGuid().ToString(), random.Next( 1, 100 ) }
			};

			var response = await this._inventoryService.AdjustProductsStockQuantitiesTo3PLWarehouse( skusQuantities, _warehouseId, CancellationToken.None );
			response.ProductsWithoutStocks.Count.Should().Be( 1 );
		}

		[ Test ]
		[ Ignore( "First make sure that product's stock doesn't exist" ) ]
		public async Task CreateProductStock()
		{
			int quantity = new Random().Next( 1, 100 );

			await this._inventoryService.CreateProductStockIn3PLWarehouse( _testProductWithoutStock, quantity, _warehouseId, CancellationToken.None );
			
			var stockInfo = await this._inventoryService.GetProductStock( _testSkuWithoutStock, _warehouseId, CancellationToken.None );
			stockInfo.Should().NotBeNull();
			stockInfo.OnHandQuantity.Should().Be( quantity );
		}

		[ Test ]
		public async Task UpdateProductsStocksWhenExceedsBatchSize()
		{
			Thread.Sleep( 10 * 1000 );
			var skusQuantities = new Dictionary< string, int >();
			var random = new Random();
			var i = 1;
			
			while ( i <= base.Config.UpdateProductStockBatchSize * 3 + 1 )
			{
				skusQuantities.Add( "SB-testsku" + i.ToString(), random.Next( 1, 100 ) );
				++i;
			}

			var response = await this._inventoryService.AdjustProductsStockQuantitiesTo3PLWarehouse( skusQuantities, _warehouseId, CancellationToken.None );
			response.ProductsWithoutStocks.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsStock()
		{
			var productsStock = await this._inventoryService.GetProductsStock( _warehouseId, CancellationToken.None );
			productsStock.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetProductsStockBySmallestPage()
		{
			base.Config.RetrieveProductsStocksTotalPageSize = 1;
			var productsStock = await this._inventoryService.GetProductsStock( _warehouseId, CancellationToken.None );
			productsStock.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task UpdateSkuQuantityToInhouseWarehouse()
		{
			Thread.Sleep( 10 * 1000 );
			int newQuantity = new Random().Next( 1, 100 );
			string locationName = "AB-C-D";

			await this._inventoryService.AdjustProductStockQuantityToInHouseWarehouse( _testSku1, newQuantity, _inHouseWarehouseId, locationName, CancellationToken.None );
			
			var detailedStock = await this._inventoryService.GetDetailedProductStock( _testSku1, _inHouseWarehouseId, CancellationToken.None );
			detailedStock.FirstOrDefault( s => s.LocationName == locationName ).OnHandQuantity.Should().Be( newQuantity );
		}

		[ Test ]
		public async Task UpdateSkuQuantityToInHouseWarehouseLocationThatDoesntExist()
		{
			Thread.Sleep( 10 * 1000 );
			int newQuantity = new Random().Next( 1, 100 );
			string locationName = Guid.NewGuid().ToString();

			var response = await this._inventoryService.AdjustProductStockQuantityToInHouseWarehouse( _testSku1, newQuantity, _inHouseWarehouseId, locationName, CancellationToken.None );
			response.ProductsWithoutStocks.Count.Should().Be( 1 );
		}

		[ Test ]
		public async Task CreateProductStockWithLocation()
		{
			int quantity = new Random().Next( 1, 100 );
			var newLocationName = Guid.NewGuid().ToString();

			await this._inventoryService.CreateProductStockInInHouseWarehouse( _testProductWithoutStock, quantity, _inHouseWarehouseId, newLocationName, CancellationToken.None );
			
			var stockInfo = await this._inventoryService.GetDetailedProductStock( _testSkuWithoutStock, _inHouseWarehouseId, CancellationToken.None );
			
			var detailedStock = await this._inventoryService.GetDetailedProductStock( _testSkuWithoutStock, _inHouseWarehouseId, CancellationToken.None );
			detailedStock.FirstOrDefault( s => s.LocationName == newLocationName ).OnHandQuantity.Should().Be( quantity );
		}
	}
}