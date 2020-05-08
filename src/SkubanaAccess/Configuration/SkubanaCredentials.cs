using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkubanaAccess.Configuration
{
	public class SkubanaUserCredentials
	{
		public string AccessToken { get; private set; }

		public SkubanaUserCredentials( string accessToken )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrEmpty();

			this.AccessToken = accessToken;
		}
	}
}