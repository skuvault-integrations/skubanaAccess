using SkubanaAccess.Authentication.Services;
using SkubanaAccess.Configuration;
using SkubanaAccess.Services.Authentication;

namespace SkubanaAccess
{
	public class SkubanaFactory : ISkubanaFactory
	{
		public IAuthenticationService CreateAuthenticationService( SkubanaConfig config )
		{
			return new AuthenticationService( config );
		}
	}
}