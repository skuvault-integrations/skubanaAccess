using Newtonsoft.Json;

namespace SkubanaAccess.Models.Response
{
	public class GetAccessTokenResponse
	{
		[ JsonProperty( "access_token" ) ]
		public string AccessToken { get; set; }
		[ JsonProperty( "token_type" ) ]
		public string TokenType { get; set; }
		[ JsonProperty( "scope" ) ]
		public string Scope { get; set; }
	}
}