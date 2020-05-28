using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class ProductStock
	{
		[ JsonProperty( "product" ) ]
		public Product Product { get; set; }
		[ JsonProperty( "onHandQuantity" ) ]
		public int OnHandQuantity { get; set; }
		[ JsonProperty( "lockedQuantity" ) ]
		public int LockedQuantity { get; set; }
		[ JsonProperty( "inTransitQuantity" ) ]
		public int InTransitQuantity { get; set; }
	}

	public class DetailedProductStock
	{
		[ JsonProperty( "product" ) ]
		public Product Product { get; set; }
		[ JsonProperty( "quantity" ) ]
		public int Quantity { get; set; }
		[ JsonProperty( "stockLocation" ) ]
		public ProductStockLocation Location { get; set; }
	}

	public class ProductStockLocation
	{
		[ JsonProperty( "location" ) ]
		public string LocationName { get; set; }
		[ JsonProperty( "stockLocationId" ) ]
		public long LocationId { get; set; }
		[ JsonProperty( "warehouseId" ) ]
		public long WarehouseId { get; set; }
	}

	public class SkubanaProductStock
	{
		public long ProductId { get; set; }
		public string ProductSku { get; set; }
		public int OnHandQuantity { get; set; }
		public string LocationName { get; set; }
	}
}