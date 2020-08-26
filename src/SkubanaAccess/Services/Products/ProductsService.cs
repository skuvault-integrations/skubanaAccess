using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Products
{
	public class ProductsService : ServiceBaseWithTokenAuth, IProductsService
	{
		public ProductsService( SkubanaConfig config ) : base( config )
		{
		}

		public async Task< IEnumerable< SkubanaProduct > > GetProductsBySkus( IEnumerable< string > skus, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Retrieve products request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var chunks = skus.SplitToChunks( base.Config.RetrieveProductsBySkusBatchSize );
			var result = new List< SkubanaProduct >();

			using( var throttler = new Throttler( 3, 12 ) )	//Rate limit 3 / 10 seconds, no hourly limit
			{
				foreach( var chunk in chunks )
				{
					var command = new RetrieveProductsCommand( base.Config, chunk )
					{
						Throttler = throttler
					};
					var response = await base.GetAsync< IEnumerable< Product > >( command, token, mark );

					if ( response != null )
					{
						// Skubana product search isn't strict
						var products = response.Select( r => r.ToSVProduct() ).Where( p => chunk.Any( c => c.ToLower().Equals( p.Sku.ToLower() ) ) ).ToList();
						result.AddRange( products );
					}
				}
			}
			
			return result;
		}

		public async Task< IEnumerable< SkubanaProduct > > GetProductsUpdatedAfterAsync( DateTime updatedAfterUtc, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Retrieve updated products request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			var result = new List< SkubanaProduct >();

			using( var throttler = new Throttler( 5, 1 ) )
			{
				var page = 1;
				while( true )
				{
					var command = new RetrieveProductsUpdatedAfterCommand( base.Config, updatedAfterUtc, page, base.Config.RetrieveProductsPageSize )
					{
						Throttler = throttler
					};

					var response = await base.GetAsync< IEnumerable< Product > >( command, token, mark ).ConfigureAwait( false );

					if ( response == null || !response.Any() )
					{
						break;
					}

					result.AddRange( response.Where( p => p.Type == ProductType.CoreProduct.Type )
										.Select( r => r.ToSVProduct() ) );
					++page;
				}
			}

			return result;
		}
	}
}