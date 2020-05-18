using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using System.Collections.Generic;

namespace SkubanaAccess.Models.Commands
{
	public class GetAccessTokenCommand : SkubanaCommand
	{
		public GetAccessTokenCommand( SkubanaConfig config, string redirectUrl, string code ) : base( config, SkubanaEndpoint.GetAccessTokenUrl, null, null, isApiCommand: false )
		{
			Condition.Requires( redirectUrl, "redirectUrl" ).IsNotNullOrEmpty();
			Condition.Requires( code, "code" ).IsNotNullOrEmpty();

			this.RequestParameters = new Dictionary< string, string >()
			{
				{ "grant_type", "authorization_code" },
				{ "redirect_uri", redirectUrl },
				{ "code", code }
			};
		}
	}
}