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
		public InventoryService( SkubanaConfig config ) : base( config )
		{
		}

		/// <summary>
		///	Adjust products stocks quantities, will override existing quantities.
		///	This endpoint should be used carefully as it might override pending activity.
		/// </summary>
		/// <param name="skusQuantities"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< AdjustProductStockQuantityResponse > AdjustProductsStockQuantities( Dictionary< string, int > skusQuantities, long warehouseId, CancellationToken token, Mark mark = null )
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
								result.ProductsWithoutStocks.Add( error.Identifier );
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
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="quantity"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< AdjustProductStockQuantityResponse > AdjustProductStockQuantity( string sku, int quantity, long warehouseId, CancellationToken token, Mark mark = null )
		{
			var skusQuantities = new Dictionary< string, int >()
			{
				{ sku, quantity }
			};

			return AdjustProductsStockQuantities( skusQuantities, warehouseId, token, mark );
		}

		/// <summary>
		///	Create products stocks with default global location
		/// </summary>
		/// <param name="productsQuantities"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task CreateProductsStock( Dictionary< long, int > productsQuantities, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Create products stocks request was cancelled", exceptionDetails ) ) );
			}

			var chunks = productsQuantities.SplitToChunks( base.Config.CreateProductStockBatchSize );

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
		///	Create product stock with default global location
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task CreateProductStock( long productId, int quantity, long warehouseId, CancellationToken token, Mark mark = null )
		{
			var productsQuantities = new Dictionary< long, int >()
			{
				{ productId, quantity }
			};

			return CreateProductsStock( productsQuantities, warehouseId, token, mark );
		}

		public async Task< IEnumerable< ProductStock > > GetProductsStock( long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Get products stock total request was cancelled", exceptionDetails ) ) );
			}

			var productsStock = new List< ProductStock >();
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
					productsStock.AddRange( pageData );
				}
			}

			return productsStock;
		}

		/// <summary>
		///	Retrieve products stock totals. 
		///	This is the physical inventory count present (e.g. it does not include potentially buildable bundles or kits). 
		///	In transit quantity does not include pending Purchase Orders.
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseId"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< ProductStock > GetProductStock( string sku, long warehouseId, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Retrieve product stock total request was cancelled", exceptionDetails ) ) );
			}

			using( var command = new GetProductStockTotalCommand( base.Config, sku, warehouseId ){ Throttler = new Throttler( 10, 1 ) } )
			{
				var stock = await base.GetAsync< IEnumerable< ProductStock > >( command, token, mark ).ConfigureAwait( false );
				return stock.FirstOrDefault();
			}
		}
	}
}