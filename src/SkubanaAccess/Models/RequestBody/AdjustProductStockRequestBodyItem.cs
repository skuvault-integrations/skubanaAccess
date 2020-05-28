using Newtonsoft.Json;

namespace SkubanaAccess.Models.RequestBody
{
	public class AdjustProductStockRequestBodyItem
	{
		[ JsonProperty( "masterSku" ) ]
		public string MasterSku { get; set; }
		[ JsonProperty( "onHandQuantity" ) ]
		public int OnHandQuantity { get; set; }
		[ JsonProperty( "warehouseId" ) ]
		public long WarehouseId { get; set; }
		[ JsonProperty( "stockLocationName" ) ]
		public string StockLocationName { get; set; }
	}
}