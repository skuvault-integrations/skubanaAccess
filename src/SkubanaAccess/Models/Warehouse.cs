using Newtonsoft.Json;
using System;

namespace SkubanaAccess.Models
{
	public class Warehouse
	{
		[ JsonProperty( "warehouseId" ) ]
		public long Id { get; set; }
		[ JsonProperty( "active" ) ]
		public bool IsActive { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "type" ) ]
		public string Type { get; set; }
	}

	public class SkubanaWarehouse
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public SkubanaWarehouseTypeEnum Type { get; set; }
	}

	public enum SkubanaWarehouseTypeEnum
	{
		DIRECT_FULFILLMENT,
		THIRD_PARTY_LOGISTICS
	}

	public static class WarehouseExtensions
	{
		public static SkubanaWarehouse ToSVWarehouse( this Warehouse warehouse )
		{
			var svWarehouse = new SkubanaWarehouse()
			{
				Id = warehouse.Id,
				Name = warehouse.Name,
				IsActive = warehouse.IsActive,
				Type = SkubanaWarehouseTypeEnum.DIRECT_FULFILLMENT
			};

			if ( Enum.TryParse< SkubanaWarehouseTypeEnum >( warehouse.Type, out SkubanaWarehouseTypeEnum warehouseType ) )
			{
				svWarehouse.Type = warehouseType;
			}

			return svWarehouse;
		}
	}
}