using System.Threading;
using System.Threading.Tasks;
using SkubanaAccess.Configuration;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Models.Response;
using SkubanaAccess.Services;
using SkubanaAccess.Services.Authentication;

namespace SkubanaAccess.Authentication.Services
{
	public class AuthenticationService : ServiceBase, IAuthenticationService
	{
		public AuthenticationService( SkubanaConfig config ) : base( config )
		{
		}

		public Task< GetAccessTokenResponse > GetAccessTokenAsync( SkubanaAppCredentials appCredentials, string code, string cid, CancellationToken token )
		{
			var command = new GetAccessTokenCommand( base.Config, appCredentials.RedirectUrl, code, cid );
			return base.PostAsync< GetAccessTokenResponse >( command, token, useBasicAuth: true, appCredentials: appCredentials );
		}

		public string GetAppInstallationUrl( SkubanaAppCredentials appCredentials )
		{
			return $"{ base.Config.Environment.BaseUrl }/oauth/authorize?client_id={ appCredentials.ApplicationKey }&scope={ string.Join( "+", appCredentials.Scopes ) }&redirect_uri={ appCredentials.RedirectUrl }&response_type=code";
		}
	}
}