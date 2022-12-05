using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Models.Response;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;

namespace SkubanaAccess.Services.Inventory
{
	public class InventoryService : ServiceBaseWithTokenAuth, IInventoryService
	{
		public const string Warehouse3PLLocationName = "GLOBAL";

		public InventoryService( SkubanaConfig config ) : base( config )
		{
		}

		/// <summary>
		///	Adjust products stocks quantities, will override existing quantities.
		///	This endpoint should be used carefully as it might override pending activity.
		///	Product's stock should already exists.
		/// </summary>
		/// <param name="skusQuantitiesByLocation"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		private async Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantities( IEnumerable< SkubanaProductStock > skusQuantities, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Adjust products stocks quantities request was cancelled", exceptionDetails ) ) );
			}

			var chunks = skusQuantities.SplitToChunks( this.Config.UpdateProductStockBatchSize );
			var result = new AdjustProductStockQuantityResponse();

			using( var throttler = new Throttler( 1, 10 ) )
			{
				foreach( var chunk in chunks )
				{
					var command = new AdjustProductsStockCommand( base.Config, chunk, warehouseId )
					{
						Throttler = throttler
					};
					var response = await base.PostAsync< SkubanaResponse< UpdatedProductStockInfo > >( command, token, mark ).ConfigureAwait( false );

					if ( response.Errors != null && response.Errors.Any() )
					{
						var criticalErrors = new List< SkubanaResponseError >();

						foreach( var error in response.Errors )
						{
							if ( error.ErrorCode == SkubanaValidationErrors.ProductStockNotFound.Code )
							{
								var locationWithoutStock = chunk[ error.Index - 1 ].LocationName;

								if ( result.ProductsWithoutStocks.TryGetValue( chunk[ error.Index - 1 ].ProductSku, out List< string > locations ) )
								{
									locations.Add( locationWithoutStock );
								}
								else
								{
									result.ProductsWithoutStocks.Add( chunk[ error.Index - 1 ].ProductSku, new List< string >(){ { locationWithoutStock } } );
								}
							}
							else if ( error.ErrorCode == SkubanaValidationErrors.ProductDoesNotExist.Code )
							{
								result.ProductsNotExist.Add( error.Identifier );
							}
							else
							{
								criticalErrors.Add( error );
							}
						}

						if ( criticalErrors.Any() )
							throw new SkubanaException( criticalErrors.ToJson() );
					}
				}
			}

			return result;
		}

		/// <summary>
		///	Adjust product stock quantities, will override existing quantities.
		///	This endpoint should be used carefully as it might override pending activity.
		///	Execute this method only against 3PL warehouse without locations.
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="quantity"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< AdjustProductStockQuantityResponse > AdjustProductStockQuantityTo3PLWarehouse( string sku, int quantity, long warehouseId, CancellationToken token, Mark mark = null )
		{
			var skusQuantities = new List< SkubanaProductStock >()
			{
				{ new SkubanaProductStock() { ProductSku = sku, LocationName = Warehouse3PLLocationName, OnHandQuantity = quantity } }
			};

			return AdjustProductsStockQuantities( skusQuantities, warehouseId, token, mark );
		}

		/// <summary>
		///	Adjust product stock quantities, will override existing quantities.
		///	This endpoint should be used carefully as it might override pending activity.
		///	Execute this method only against 3PL warehouse without locations.
		/// </summary>
		/// <param name="skusQuantities"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantitiesTo3PLWarehouse( Dictionary< string, int > skusQuantities, long warehouseId, CancellationToken token, Mark mark = null )
		{
			var skusQuantitiesWithGlobalLocation = new List< SkubanaProductStock >();

			foreach( var skuQuantity in skusQuantities )
			{
				skusQuantitiesWithGlobalLocation.Add( new SkubanaProductStock() { ProductSku = skuQuantity.Key, LocationName = Warehouse3PLLocationName, OnHandQuantity = skuQuantity.Value } );
			}

			return AdjustProductsStockQuantities( skusQuantitiesWithGlobalLocation, warehouseId, token, mark );
		}

		/// <summary>
		///	Adjust product stock quantities, will override existing quantities.
		///	This endpoint should be used carefully as it might override pending activity.
		///	Execute this method only against InHouse warehouse with locations.
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="quantity"></param>
		/// <param name="warehouseId"></param>
		/// <param name="locationName"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< AdjustProductStockQuantityResponse > AdjustProductStockQuantityToInHouseWarehouse( string sku, int quantity, long warehouseId, string locationName, CancellationToken token, Mark mark = null )
		{
			var skusQuantities = new List< SkubanaProductStock >()
			{
				{ new SkubanaProductStock() { ProductSku = sku, LocationName = locationName, OnHandQuantity = quantity } }
			};

			return AdjustProductsStockQuantities( skusQuantities, warehouseId, token, mark );
		}

		/// <summary>
		///	Adjust products stocks quantities, will override existing quantities.
		///	This endpoint should be used carefully as it might override pending activity.
		///	Product's stock should already exists.
		/// </summary>
		/// <param name="skusQuantitiesByLocation"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantitiesToInHouseWarehouse( IEnumerable< SkubanaProductStock > skusQuantitiesByLocation, long warehouseId, CancellationToken token, Mark mark = null )
		{
			return AdjustProductsStockQuantities( skusQuantitiesByLocation, warehouseId, token, mark );
		}

		/// <summary>
		///	Create products stocks
		/// </summary>
		/// <param name="productsQuantities"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		private async Task CreateProductsStock( IEnumerable< SkubanaProductStock > productsStock, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Create products stocks request was cancelled", exceptionDetails ) ) );
			}

			var chunks = productsStock.SplitToChunks( base.Config.CreateProductStockBatchSize );

			using( var throttler = new Throttler( 1, 30 ) )
			{
				foreach( var chunk in chunks )
				{
					var command = new CreateProductsStockCommand( base.Config, chunk, warehouseId )
					{
						Throttler = throttler
					};
					var response = await base.PutAsync< SkubanaResponse< UpdatedProductStockInfo > >( command, token, mark ).ConfigureAwait( false );

					if ( response.Errors != null && response.Errors.Any() )
					{
						throw new SkubanaException( response.Errors.ToJson() );
					}
				}
			}
		}

		/// <summary>
		///	Create product stock in 3PL warehouse with default global location
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task CreateProductStockIn3PLWarehouse( long productId, int quantity, long warehouseId, CancellationToken token, Mark mark = null )
		{
			var productsQuantities = new List< SkubanaProductStock >()
			{
				{ new SkubanaProductStock() { ProductId = productId, LocationName = Warehouse3PLLocationName, OnHandQuantity = quantity } }
			};

			return CreateProductsStock( productsQuantities, warehouseId, token, mark );
		}

		/// <summary>
		///	Create products stocks in 3PL warehouse with default global location
		/// </summary>
		/// <param name="productsQuantities"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task CreateProductsStockIn3PLWarehouse( Dictionary< long, int > productsQuantities, long warehouseId, CancellationToken token, Mark mark = null )
		{
			var productsStock = new List< SkubanaProductStock >();
			foreach( var productQuantity in productsQuantities )
			{
				productsStock.Add( new SkubanaProductStock() { ProductId = productQuantity.Key, OnHandQuantity = productQuantity.Value, LocationName = Warehouse3PLLocationName } );
			}

			return CreateProductsStock( productsStock, warehouseId, token, mark );
		}

		/// <summary>
		///	
		/// </summary>
		/// <param name="productsStocks"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task CreateProductStockInInHouseWarehouse( long productId, int quantity, long warehouseId, string locationName, CancellationToken token, Mark mark = null )
		{
			var productsQuantities = new List< SkubanaProductStock >()
			{
				{ new SkubanaProductStock() { ProductId = productId, LocationName = locationName, OnHandQuantity = quantity } }
			};

			return CreateProductsStock( productsQuantities, warehouseId, token, mark );
		}

		/// <summary>
		///	Create products stock in InHouse warehouse
		/// </summary>
		/// <param name="productsStocks"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task CreateProductsStockInInHouseWarehouse( IEnumerable< SkubanaProductStock > productsStocks, long warehouseId, CancellationToken token, Mark mark = null )
		{
			return CreateProductsStock( productsStocks, warehouseId, token, mark );
		}

		/// <summary>
		///	Retrieve products stock totals. 
		///	This is the physical inventory count present (e.g. it does not include potentially buildable bundles or kits). 
		///	In transit quantity does not include pending Purchase Orders.
		/// </summary>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< SkubanaProductStock > > GetProductsStock( long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get products stock total request was cancelled", exceptionDetails ) ) );
			}

			var productsStock = new List< SkubanaProductStock >();
			int pageIndex = 1;

			using( var throttler = new Throttler( 10, 1 ) )
			{
				while( true )
				{
					var command = new RetrieveProductsStocksTotalCommand( base.Config, warehouseId, pageIndex, base.Config.RetrieveProductsStocksTotalPageSize )
					{
						Throttler = throttler
					};

					var pageData = await base.GetAsync< IEnumerable< ProductStock > >( command, token, mark ).ConfigureAwait( false );

					if ( pageData == null || !pageData.Any() )
					{
						break;
					}

					++pageIndex;
					productsStock.AddRange( pageData.Select( p => new SkubanaProductStock() { ProductId = p.Product.Id, ProductSku = p.Product.Sku, OnHandQuantity = p.OnHandQuantity } ) );
				}
			}

			return productsStock;
		}

		/// <summary>
		///	Retrieve product stock.
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< SkubanaProductStock > GetProductStock( string sku, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Retrieve product stock total request was cancelled", exceptionDetails ) ) );
			}

			using( var throttler = new Throttler( 10, 1 ) )
			{
				using( var command = new GetProductStockTotalCommand( base.Config, sku, warehouseId ){ Throttler = throttler } )
				{
					var stock = await base.GetAsync< IEnumerable< ProductStock > >( command, token, mark ).ConfigureAwait( false );
					return stock.Select( s => new SkubanaProductStock() { ProductId = s.Product.Id, ProductSku = s.Product.Sku, OnHandQuantity = s.OnHandQuantity } ).FirstOrDefault();
				}
			}
		}

		/// <summary>
		///	Retrieve product stock broke down by location
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< SkubanaProductStock > > GetDetailedProductStock( string sku, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Retrieve product detailed stock request was cancelled", exceptionDetails ) ) );
			}

			using( var throttler = new Throttler( 5, 1 ) )
			{
				using( var command = new GetProductStockCommand( base.Config, sku, warehouseId ){ Throttler = throttler } )
				{
					var stock = await base.GetAsync< IEnumerable< DetailedProductStock > >( command, token, mark ).ConfigureAwait( false );
					// Need to filter out products with skus that do not exactly match the input sku since the endpoint originally does include them.
					var filteredStock = stock.Where( s => s.Product.Sku.Equals( sku, StringComparison.OrdinalIgnoreCase ) );
					return filteredStock.Select( s => new SkubanaProductStock() { ProductId = s.Product.Id, ProductSku = s.Product.Sku, OnHandQuantity = s.Quantity, LocationName = s.Location.LocationName } );
				}
			}
		}
	}
}