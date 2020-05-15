using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Authentication.Services;
using SkubanaAccess.Configuration;
using SkubanaAccess.Services.Authentication;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class AuthenticationTests : BaseTest
	{
		private IAuthenticationService _service;
		
		[ SetUp ]
		public void Init()
		{
			this._service = new AuthenticationService( base.Config, base.AppCredentials );
		}

		[ Test ]
		public void GetSandboxAppInstallationUrl()
		{
			base.Config.Environment = SkubanaEnvironment.Sandbox;
			var appAuthUrl = this._service.GetAppInstallationUrl();

			appAuthUrl.Should().NotBeNullOrEmpty();
			appAuthUrl.Should().StartWith( base.Config.Environment.BaseAuthUrl );
		}

		[ Test ]
		public void GetProductionAppInstallationUrl()
		{
			base.Config.Environment = SkubanaEnvironment.Production;
			var appAuthUrl = this._service.GetAppInstallationUrl();

			appAuthUrl.Should().NotBeNullOrEmpty();
			appAuthUrl.Should().StartWith( base.Config.Environment.BaseAuthUrl );
		}

		[ Test ]
		public async Task GetAccessToken()
		{
			var code = "EPu5t1";

			var response = await this._service.GetAccessTokenAsync( code, CancellationToken.None );
			response.Should().NotBeNull();
			response.AccessToken.Should().NotBeNullOrEmpty();
		}
	}
}