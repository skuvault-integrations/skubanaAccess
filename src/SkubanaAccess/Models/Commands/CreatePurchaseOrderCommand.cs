using System.Collections.Generic;
using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Models.RequestBody;

namespace SkubanaAccess.Models.Commands
{
	public class CreatePurchaseOrderCommand : SkubanaCommand
	{
		public CreatePurchaseOrderCommand( SkubanaConfig config, SkubanaPurchaseOrder purchaseOrder,
			IDictionary< string, long > vendorProductIdsBySku ) : base( config, SkubanaEndpoint.CreatePurchaseOrderUrl )
		{
			Condition.Requires( purchaseOrder, "purchaseOrder" ).IsNotNull();
			Condition.Requires( vendorProductIdsBySku, "vendorProductIdsBySku" ).IsNotEmpty();

			this.Payload = purchaseOrder.ToCreatePurchaseOrderRequestBody( vendorProductIdsBySku );
		}
	}
}
