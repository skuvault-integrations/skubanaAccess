using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkubanaAccess.Configuration;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Models.Response;
using SkubanaAccess.Services;
using SkubanaAccess.Services.Authentication;

namespace SkubanaAccess.Authentication.Services
{
	public class AuthenticationService : ServiceBaseWithBasicAuth, IAuthenticationService
	{
		public AuthenticationService( SkubanaConfig config, SkubanaAppCredentials appCredentials ) : base( config, appCredentials )
		{
		}

		public Task< GetAccessTokenResponse > GetAccessTokenAsync( string code, CancellationToken token )
		{
			using( var command = new GetAccessTokenCommand( base.Config, AppCredentials.RedirectUrl, code ) )
			{
				return base.PostAsync< GetAccessTokenResponse >( command, token );
			}
		}

		public string GetAppInstallationUrl()
		{
			return $"{ base.Config.Environment.BaseAuthUrl }/oauth/authorize?client_id={ AppCredentials.ApplicationKey }&scope={ string.Join( "+", AppCredentials.Scopes.Select( s => s.ToString().ToLower() ) ) }&redirect_uri={ AppCredentials.RedirectUrl }&response_type=code";
		}
	}
}