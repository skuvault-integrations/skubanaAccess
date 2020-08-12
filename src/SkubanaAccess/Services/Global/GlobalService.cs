using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Global
{
	public class GlobalService : ServiceBaseWithTokenAuth, IGlobalService
	{
		public GlobalService( SkubanaConfig config ) : base( config )
		{
		}

		public async Task< IEnumerable< SkubanaWarehouse > > ListWarehouses( CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. List warehouses request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			using( var command = new ListWarehousesCommand( base.Config ) )
			{
				var response = await base.GetAsync< IEnumerable< Warehouse > >( command, token, mark );
				return response.Select( r => r.ToSVWarehouse() );
			}
		}

		public async Task< SkubanaWarehouse > GetWarehouseByIdAsync( long warehouseId, CancellationToken token, Netco.Logging.Mark mark )
		{
			var skubanaMark = new Mark( mark.MarkValue );

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.Environment.BaseApiUrl, skubanaMark, additionalInfo: this.AdditionalLogInfo() );
				var exception = new SkubanaException( string.Format( "{0}. Get warehouse by id request was cancelled", exceptionDetails ) );
				SkubanaLogger.LogTraceException( exception );
				throw exception;
			}

			using ( var command = new GetWarehouseByIdCommand( base.Config, warehouseId ) )
			{
				var response = await base.GetAsync< Warehouse >( command, token, skubanaMark );
				return response.ToSVWarehouse();
			}
		}
	}
}