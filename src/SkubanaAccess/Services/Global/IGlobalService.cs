using SkubanaAccess.Models;
using SkubanaAccess.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Global
{
	public interface IGlobalService : IDisposable
	{
		Task< IEnumerable< SkubanaWarehouse > > ListWarehouses( CancellationToken token, Mark mark = null );
	}
}