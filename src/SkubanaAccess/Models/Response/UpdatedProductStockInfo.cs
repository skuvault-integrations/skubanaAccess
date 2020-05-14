using Newtonsoft.Json;

namespace SkubanaAccess.Models.Response
{
	public class UpdatedProductStockInfo
	{
		[ JsonProperty( "masterSku" ) ]
		public string MasterSku { get; set; }
		[ JsonProperty( "availableQuantity" ) ]
		public int AvailableQuantity { get; set; }
		[ JsonProperty( "productStockId" ) ]
		public long? ProductStockId { get; set; }
		[ JsonProperty( "warehouseId" ) ]
		public long WarehouseId { get; set; }
	}
}