using SkubanaAccess.Configuration;
using SkubanaAccess.Models.Response;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Authentication
{
	public interface IAuthenticationService
	{
		string GetAppInstallationUrl();
		Task< GetAccessTokenResponse > GetAccessTokenAsync( string code, CancellationToken token );
	}
}