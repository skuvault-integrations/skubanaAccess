using SkubanaAccess.Models;
using SkubanaAccess.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services.Global
{
	public interface IGlobalService
	{
		Task< IEnumerable< SkubanaWarehouse > > ListWarehouses( CancellationToken token, Mark mark = null );
	}
}