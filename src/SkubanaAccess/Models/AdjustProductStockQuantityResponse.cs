using System.Collections.Generic;

namespace SkubanaAccess.Models
{
	public class AdjustProductStockQuantityResponse
	{
		public AdjustProductStockQuantityResponse()
		{
			this.ProductsWithoutStocks = new List< string >();
			this.ProductsNotExist = new List< string >();
		}

		public List< string > ProductsWithoutStocks { get; private set; }
		public List< string > ProductsNotExist { get; private set; }
	}
}