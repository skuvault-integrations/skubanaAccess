using FluentAssertions;
using NUnit.Framework;
using SkubanaAccess.Services.Global;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccessTests
{
	[ TestFixture ]
	public class GlobalTests : BaseTest
	{
		private IGlobalService _globalService;

		[ SetUp ]
		public void Init()
		{
			this._globalService = new GlobalService( base.Config );
		}

		[ Test ]
		public async Task ListWarehouses()
		{
			var warehouses = await this._globalService.ListWarehouses( CancellationToken.None );
			warehouses.Should().NotBeNullOrEmpty();
		}
	}
}