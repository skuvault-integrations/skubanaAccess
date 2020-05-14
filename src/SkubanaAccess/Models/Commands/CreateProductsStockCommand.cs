using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Models.RequestBody;
using SkubanaAccess.Throttling;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class CreateProductsStockCommand : SkubanaCommand
	{
		/// <summary>
		///	3PL warehouses can have only one global stock location
		/// </summary>
		private const string ExternalWarehouseDefaultStockLocationName = "GLOBAL";
		private const int maxQuantity = 100 * 1000 * 1000;

		public CreateProductsStockCommand( SkubanaConfig config, Dictionary< long, int > productsQuantities, long warehouseId ) : base( config, SkubanaEndpoint.CreateProductStockUrl )
		{
			Condition.Requires( productsQuantities, "productsQuantities" ).IsNotNull().IsNotEmpty();
			Condition.Requires( warehouseId, "warehouseId" ).IsGreaterThan( 0 );

			var body = new List< CreateProductStockRequestBodyItem >();
			foreach( var productQuantity in productsQuantities )
			{
				body.Add( new CreateProductStockRequestBodyItem()
				{
					ProductId = productQuantity.Key,
					Quantity = productQuantity.Value > maxQuantity ? maxQuantity : productQuantity.Value,
					WarehouseId = warehouseId,
					StockLocationName = ExternalWarehouseDefaultStockLocationName
				} );
			};

			this.Payload = body;
		}
	}
}