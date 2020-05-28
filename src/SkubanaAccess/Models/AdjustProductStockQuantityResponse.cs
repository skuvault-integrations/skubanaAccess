using System.Collections.Generic;

namespace SkubanaAccess.Models
{
	public class AdjustProductStockQuantityResponse
	{
		public AdjustProductStockQuantityResponse()
		{
			this.ProductsWithoutStocks = new Dictionary< string, List< string > >();
			this.ProductsNotExist = new List< string >();
		}

		public Dictionary< string, List< string > > ProductsWithoutStocks { get; private set; }
		public List< string > ProductsNotExist { get; private set; }
	}
}