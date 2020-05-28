using Newtonsoft.Json;

namespace SkubanaAccess.Models
{
	public class Money
	{
		[ JsonProperty( "amount" ) ]
		public decimal Amount { get; set; }
		[ JsonProperty( "currency" ) ]
		public string Currency { get; set; }
	}
}