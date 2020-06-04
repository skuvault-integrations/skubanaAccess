using Newtonsoft.Json;

namespace SkubanaAccess.Models.RequestBody
{
	public class CreateProductStockRequestBodyItem
	{
		[ JsonProperty( "minStock" ) ]
		public int MinStock { get; set; }
		[ JsonProperty( "pickable" ) ]
		public bool Pickable { get; set; }
		[ JsonProperty( "productId" ) ]
		public long ProductId { get; set; }
		[ JsonProperty( "quantity" ) ]
		public int Quantity { get; set; }
		[ JsonProperty( "quantityLocked" ) ]
		public int QuantityLocked { get; set; }
		[ JsonProperty( "receivable" ) ]
		public bool Receivable { get; set; }
		[ JsonProperty( "warehouseId" ) ]
		public long WarehouseId { get; set; }
		[ JsonProperty( "allocatedQuantity" ) ]
		public int AllocatedQuantity { get; set; }
		[ JsonProperty( "stockLocationName" ) ]
		public string StockLocationName { get; set; }
	}
}