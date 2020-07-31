using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class Vendor
	{
		[ JsonProperty( "vendorId" ) ]
		public long VendorId { get; set; }

		[ JsonProperty( "supplier" ) ]
		public bool IsSupplier { get; set; }

		[ JsonProperty( "active" ) ]
		public bool IsActive { get; set; }
		
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
	}

	public class SkubanaVendor
	{
		public string Name { get; set; }
	}
}
