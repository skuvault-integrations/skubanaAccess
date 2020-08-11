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

		[ JsonProperty( "defaultVendorPaymentTerm" ) ]
		public VendorPaymentTerm DefaultVendorPaymentTerm { get; set; }

		[ JsonProperty( "defaultIncotermShippingRuleId" ) ]
		public long DefaultIncotermShippingRuleId { get; set; }

		[ JsonProperty( "defaultPurchaseOrderTemplateId" ) ]
		public long DefaultPurchaseOrderTemplateId { get; set; }
	}

	public class SkubanaVendor
	{
		public long VendorId { get; set; }
		public string Name { get; set; }
		public long? PaymentTermId { get; set; }
		public long DefaultIncotermShippingRuleId { get; set; }
		public long DefaultPurchaseOrderTemplateId { get; set; }
	}

	public static class VendorExtensions
	{
		public static SkubanaVendor ToSvVendor( this Vendor vendor )
		{
			return new SkubanaVendor
			{
				VendorId = vendor.VendorId,
				Name = vendor.Name,
				PaymentTermId = vendor.DefaultVendorPaymentTerm?.VendorPaymentTermId,
				DefaultIncotermShippingRuleId = vendor.DefaultIncotermShippingRuleId,
				DefaultPurchaseOrderTemplateId = vendor.DefaultPurchaseOrderTemplateId
			};
		}
	}
}
