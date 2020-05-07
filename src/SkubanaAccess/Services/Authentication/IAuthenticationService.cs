using SkubanaAccess.Configuration;
using SkubanaAccess.Models.Response;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Authentication
{
	public interface IAuthenticationService
	{
		string GetAppInstallationUrl( SkubanaAppCredentials appCredentials );
		Task< GetAccessTokenResponse > GetAccessTokenAsync( SkubanaAppCredentials appCredentials, string code, string cid, CancellationToken token );
	}
}