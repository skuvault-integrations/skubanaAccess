using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class VendorPaymentTerm
	{
		[ JsonProperty( "vendorPaymentTermId" ) ]
		public long VendorPaymentTermId { get; set; }

		[ JsonProperty( "description" ) ]
		public string Description { get; set; }

		[ JsonProperty( "active" ) ]
		public bool Active { get; set; }
	}
}
