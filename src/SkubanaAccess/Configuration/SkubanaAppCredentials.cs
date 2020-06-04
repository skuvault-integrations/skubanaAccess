using CuttingEdge.Conditions;
using System.Collections.Generic;

namespace SkubanaAccess.Configuration
{
	public class SkubanaAppCredentials
	{
		public string ApplicationKey { get; private set; }
		public string ApplicationSecret { get; private set; }
		public string RedirectUrl { get; private set; }
		public IEnumerable< SkubanaAppPermissionEnum > Scopes { get; private set; }

		public SkubanaAppCredentials( string applicationKey, string applicationSecret, string redirectUrl, IEnumerable< SkubanaAppPermissionEnum > scopes )
		{
			Condition.Requires( applicationKey, "applicationKey" ).IsNotNullOrEmpty();
			Condition.Requires( applicationSecret, "applicationSecret" ).IsNotNullOrEmpty();
			Condition.Requires( redirectUrl, "redirectUrl" ).IsNotNullOrEmpty();
			Condition.Requires( scopes, "scopes" ).IsNotEmpty();

			this.ApplicationKey = applicationKey;
			this.ApplicationSecret = applicationSecret;
			this.RedirectUrl = redirectUrl;
			this.Scopes = scopes;
		}
	}

	public enum SkubanaAppPermissionEnum
	{
		NONE,
		ALL,
		READ_INVENTORY,
		WRITE_INVENTORY,
		READ_ORDERS,
		WRITE_ORDERS,
		READ_SHIPMENTS,
		WRITE_SHIPMENTS,
		READ_PRODUCTS,
		WRITE_PRODUCTS,
		READ_PURCHASEORDERS,
		WRITE_PURCHASEORDERS,
		READ_ANALYTICS,
		READ_STOCKTRANSFERORDERS,
		WRITE_STOCKTRANSFERORDERS
	}
}