using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class GetPOsByCustomPurchaseOrderNumberCommand : SkubanaCommand
	{
		public GetPOsByCustomPurchaseOrderNumberCommand( SkubanaConfig config, string customPurchaseOrderNumber ) : base( config, SkubanaEndpoint.RetrievePurchaseOrdersUrl )
		{
			Condition.Requires( customPurchaseOrderNumber, "customPurchaseOrderNumber" ).IsNotNullOrWhiteSpace();

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "customPurchaseOrderNumber", customPurchaseOrderNumber }
			};
		}
	}
}
