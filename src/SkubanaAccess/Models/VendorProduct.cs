using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class VendorProduct
	{
		[ JsonProperty( "vendorProductId" ) ]
		public long VendorProductId { get; set; }
	}
}
