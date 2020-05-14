using SkubanaAccess.Configuration;

namespace SkubanaAccess.Models.Commands
{
	public class ListWarehousesCommand : SkubanaCommand
	{
		public ListWarehousesCommand( SkubanaConfig config ) : base( config, SkubanaEndpoint.ListWarehousesUrl )
		{
			this.Throttler = new Throttling.Throttler( 5, 1 );
		}
	}
}