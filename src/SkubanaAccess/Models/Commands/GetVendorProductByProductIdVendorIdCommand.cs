using System.Collections.Generic;
using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;

namespace SkubanaAccess.Models.Commands
{
	public class GetVendorProductByProductIdVendorIdCommand : SkubanaCommand
	{

		public GetVendorProductByProductIdVendorIdCommand( SkubanaConfig config, long productId, long vendorId ) : base( config, SkubanaEndpoint.RetrieveVendorProductsUrl )
		{
			Condition.Requires( productId, "productId" ).IsGreaterThan( default );
			Condition.Requires( vendorId, "vendorId" ).IsGreaterThan( default );

			this.RequestParameters = new Dictionary<string, string>
			{
				{ "productId", productId.ToString() },
				{ "vendorId", vendorId.ToString() },
			};
		}
	}
}
