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
				SkubanaLogger.LogTraceException( new SkubanaException( string.Format( "{0}. List warehouses request was cancelled", exceptionDetails ) ) );
			}

			using( var command = new ListWarehousesCommand( base.Config ) )
			{
				var response = await base.GetAsync< IEnumerable< Warehouse > >( command, token, mark );
				return response.Select( r => r.ToSVWarehouse() );
			}
		}
	}
}