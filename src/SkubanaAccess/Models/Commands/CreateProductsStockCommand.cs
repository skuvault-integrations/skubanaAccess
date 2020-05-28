using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Models.RequestBody;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class CreateProductsStockCommand : SkubanaCommand
	{
		private const int maxQuantity = 100 * 1000 * 1000;

		public CreateProductsStockCommand( SkubanaConfig config, IEnumerable< SkubanaProductStock > productsStock, long warehouseId ) : base( config, SkubanaEndpoint.CreateProductStockUrl )
		{
			Condition.Requires( productsStock, "productsStock" ).IsNotNull().IsNotEmpty();
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );

			var body = new List< CreateProductStockRequestBodyItem >();
			foreach( var productStock in productsStock )
			{
				body.Add( new CreateProductStockRequestBodyItem()
				{
					ProductId = productStock.ProductId,
					Quantity = productStock.OnHandQuantity > maxQuantity ? maxQuantity : productStock.OnHandQuantity,
					WarehouseId = warehouseId,
					StockLocationName = productStock.LocationName,
					Pickable = true,
					Receivable = true
				} );
			};

			this.Payload = body;
		}
	}
}