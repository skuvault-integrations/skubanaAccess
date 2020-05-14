using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Products
{
	public class ProductsService : ServiceBaseWithTokenAuth, IProductsService
	{
		public ProductsService( SkubanaConfig config ) : base( config )
		{

		}

		public async Task< IEnumerable< Product > > GetProductsBySkus( IEnumerable< string > skus, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. Retrieve products request was cancelled", exceptionDetails ) ) );
			}

			var throttler = new Throttler( 5, 1 );
			var chunks = skus.SplitToChunks( base.Config.RetrieveProductsBatchSize );
			var result = new List< Product >();

			foreach( var chunk in chunks )
			{
				var command = new RetrieveProductsCommand( base.Config, chunk )
				{
					Throttler = throttler
				};
				var response = await base.GetAsync< IEnumerable< Product > >( command, token, mark );

				if ( response != null )
				{
					result.AddRange( response );
				}
			}
			
			return result;
		}
	}
}