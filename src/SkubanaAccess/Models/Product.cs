using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class Product
	{
		[ JsonProperty( "productId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "masterSku" ) ]
		public string Sku { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "productType" ) ]
		public string Type { get; set; }
		[ JsonProperty( "upc" ) ]
		public string Upc { get; set; }
		[ JsonProperty( "mpn" ) ]
		public string Mpn { get; set; }
		[ JsonProperty( "description" ) ]
		public string Description { get; set; }
		[ JsonProperty( "hazmat" ) ]
		public bool Hazmat { get; set; }
		[ JsonProperty( "productWeight" ) ]
		public decimal Weight { get; set; }
	}
}