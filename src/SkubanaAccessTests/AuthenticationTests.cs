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
			this._service = new AuthenticationService( base.Config );
		}

		[ Test ]
		public void GetSandboxAppInstallationUrl()
		{
			base.Config.Environment = SkubanaEnvironment.Sandbox;
			var appAuthUrl = this._service.GetAppInstallationUrl( base.AppCredentials );

			appAuthUrl.Should().NotBeNullOrEmpty();
			appAuthUrl.Should().StartWith( base.Config.Environment.BaseUrl );
		}

		[ Test ]
		public void GetProductionAppInstallationUrl()
		{
			base.Config.Environment = SkubanaEnvironment.Production;
			var appAuthUrl = this._service.GetAppInstallationUrl( base.AppCredentials );

			appAuthUrl.Should().NotBeNullOrEmpty();
			appAuthUrl.Should().StartWith( base.Config.Environment.BaseUrl );
		}

		[ Test ]
		public async Task GetAccessToken()
		{
			var code = "ESD8Un";
			var cid = Guid.NewGuid().ToString();

			var response = await this._service.GetAccessTokenAsync( base.AppCredentials, code, cid, CancellationToken.None );
			response.Should().NotBeNull();
			response.AccessToken.Should().NotBeNullOrEmpty();
		}
	}
}