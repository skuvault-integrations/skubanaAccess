using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkubanaAccess.Configuration
{
	public class SkubanaCredentials
	{
		public string AccessToken { get; private set; }

		public SkubanaCredentials( string accessToken )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrEmpty();

			this.AccessToken = accessToken;
		}
	}
}