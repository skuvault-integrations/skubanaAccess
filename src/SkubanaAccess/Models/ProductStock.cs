using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class ProductStock
	{
		[ JsonProperty( "product" ) ]
		public Product Product { get; set; }
		[ JsonProperty( "onHandQuantity" ) ]
		public int OnHandQuantity { get; set; }
		[ JsonProperty( "lockedQuantity" ) ]
		public int LockedQuantity { get; set; }
		[ JsonProperty( "inTransitQuantity" ) ]
		public int InTransitQuantity { get; set; }
	}
}