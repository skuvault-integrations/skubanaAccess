using SkubanaAccess.Configuration;
using SkubanaAccess.Services.Authentication;
using SkubanaAccess.Services.Global;
using SkubanaAccess.Services.Inventory;
using SkubanaAccess.Services.Orders;
using SkubanaAccess.Services.Products;
using SkubanaAccess.Services.PurchaseOrders;

namespace SkubanaAccess
{
	public interface ISkubanaFactory
	{
		IAuthenticationService CreateAuthenticationService( SkubanaConfig config, SkubanaAppCredentials appCredentials );
		IInventoryService CreateInventoryService( SkubanaConfig config );
		IProductsService CreateProductsService( SkubanaConfig config );
		IGlobalService CreateGlobalService( SkubanaConfig config );
		IOrdersService CreateOrdersService( SkubanaConfig config );
		IPurchaseOrdersService CreatePurchaseOrdersService(SkubanaConfig config);
	}
}