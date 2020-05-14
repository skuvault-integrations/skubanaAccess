using SkubanaAccess.Configuration;
using SkubanaAccess.Services.Authentication;
using SkubanaAccess.Services.Inventory;
using SkubanaAccess.Services.Products;

namespace SkubanaAccess
{
	public interface ISkubanaFactory
	{
		IAuthenticationService CreateAuthenticationService( SkubanaConfig config, SkubanaAppCredentials appCredentials );
		IInventoryService CreateInventoryService( SkubanaConfig config );
		IProductsService CreateProductsService( SkubanaConfig config );
	}
}