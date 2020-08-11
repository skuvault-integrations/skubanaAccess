using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Models;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class VendorMapperTests
	{
		[ Test ]
		public void ToSvVendor()
		{
			const string name = "Bob";
			const long defaultIncotermShippingRuleId = 1;
			const long defaultPurchaseOrderTemplateId = 2;
			const long vendorPaymentTermId = 5;
			const long vendorId = 9;
			var vendor = new Vendor
			{
				Name = name,
				DefaultIncotermShippingRuleId = defaultIncotermShippingRuleId,
				DefaultPurchaseOrderTemplateId = defaultPurchaseOrderTemplateId,
				DefaultVendorPaymentTerm = new VendorPaymentTerm
				{
					Description = "aldfksj",
					Active = true,
					VendorPaymentTermId = vendorPaymentTermId
				},
				IsActive = true,
				IsSupplier = true,
				VendorId = vendorId
			};

			var result = vendor.ToSvVendor();

			result.Name.Should().Be( name );
			result.DefaultIncotermShippingRuleId.Should().Be( defaultIncotermShippingRuleId );
			result.DefaultPurchaseOrderTemplateId.Should().Be( defaultPurchaseOrderTemplateId );
			result.PaymentTermId.Should().Be( vendorPaymentTermId );
			result.VendorId.Should().Be( vendorId );
		}
	}
}
