using System.Collections.Generic;
using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class PurchaseOrderBase
	{
		[ JsonProperty( "assignedPurchaseOrderMilestones" ) ]
		public object AssignedPurchaseOrderMilestones { get; set; }

		[ JsonProperty( "autoUpdatesEnabled" ) ]
		public bool AutoUpdatesEnabled { get; set; }

		[ JsonProperty( "currency" ) ]
		public string Currency { get; set; }

		[ JsonProperty( "customPurchaseOrderNumber" ) ]
		public string CustomPurchaseOrderNumber { get; set; }

		[ JsonProperty( "dateAuthorized" ) ]
		public string DateAuthorized { get; set; }

		[ JsonProperty( "dateIssued" ) ]
		public string DateIssued { get; set; }

		[ JsonProperty( "destinationWarehouseId" ) ]
		public long DestinationWarehouseId { get; set; }

		[ JsonProperty( "format" ) ]
		public string Format { get; set; }

		[ JsonProperty( "incotermShippingRuleId" ) ]
		public long? IncotermShippingRuleId { get; set; }

		[ JsonProperty( "internalNotes" ) ]
		public string InternalNotes { get; set; }

		[ JsonProperty( "messagesToVendor" ) ]
		public object MessagesToVendor { get; set; }	//Not using because api always returns as empty

		[ JsonProperty( "otherCosts" ) ]
		public IEnumerable< OtherCost > OtherCosts { get; set; }

		[ JsonProperty( "purchaseOrderTemplateId" ) ]
		public long PurchaseOrderTemplateId { get; set; }
		
		[ JsonProperty( "shippingCost" ) ]
		public Money ShippingCost { get; set; }

		[ JsonProperty( "status" ) ]
		public string Status { get; set; }

		[ JsonProperty( "vendorId" ) ]
		public long VendorId { get; set; }

		[ JsonProperty( "vendorConfirmByDate" ) ]
		public string VendorConfirmByDate { get; set; }

		[ JsonProperty( "vendorConfirmedOnDate" ) ]
		public string VendorConfirmedOnDate { get; set; }

		[ JsonProperty( "type" ) ]
		public string Type { get; set; }
	}
}
