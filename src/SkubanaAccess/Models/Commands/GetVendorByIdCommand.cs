using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;

namespace SkubanaAccess.Models.Commands
{
	public class GetVendorByIdCommand : SkubanaCommand
	{
		public GetVendorByIdCommand( SkubanaConfig config, long vendorId ) : base( config, SkubanaEndpoint.GetVendorByIdUrl, routeId: vendorId.ToString() )
		{
			Condition.Requires( vendorId, "vendorId" ).IsGreaterThan( default( long ) );
		}
	}
}
