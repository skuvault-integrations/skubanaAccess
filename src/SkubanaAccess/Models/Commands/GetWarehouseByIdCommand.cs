using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;

namespace SkubanaAccess.Models.Commands
{
    public class GetWarehouseByIdCommand : SkubanaCommand
    {
	    public GetWarehouseByIdCommand( SkubanaConfig config, long warehouseId ) : base( config, SkubanaEndpoint.GetWarehouseByIdUrl, routeId: warehouseId.ToString() )
	    {
		    Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( default( long ) );
	    }
    }
}
