using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace SkubanaAccess.Models.Commands
{
	public class RetrieveProductsCommand : SkubanaCommand
	{
		public RetrieveProductsCommand( SkubanaConfig config, IEnumerable< string > skus ) : base( config, SkubanaEndpoint.RetrieveProductsUrl )
		{
			Condition.Requires( skus, "skus" ).IsNotNull().IsNotEmpty();

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "sku", string.Join( ",", skus.Select( s => $"\"{ s }\"" ) ) }
			};
			this.Throttler = new Throttling.Throttler( 5, 1 );
		}
	}
}