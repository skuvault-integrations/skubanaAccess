using CuttingEdge.Conditions;
using System.Collections.Generic;

namespace SkubanaAccess.Configuration
{
	public class SkubanaAppCredentials
	{
		public string ApplicationKey { get; private set; }
		public string ApplicationSecret { get; private set; }
		public string RedirectUrl { get; private set; }
		public IEnumerable< string > Scopes { get; private set; }

		public SkubanaAppCredentials( string applicationKey, string applicationSecret, string redirectUrl, IEnumerable< string > scopes )
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
}