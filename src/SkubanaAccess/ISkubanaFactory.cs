using SkubanaAccess.Configuration;
using SkubanaAccess.Services.Authentication;

namespace SkubanaAccess
{
	public interface ISkubanaFactory
	{
		IAuthenticationService CreateAuthenticationService( SkubanaConfig config );
	}
}