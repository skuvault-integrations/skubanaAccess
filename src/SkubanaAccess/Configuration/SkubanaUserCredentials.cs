using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkubanaAccess.Configuration
{
	public class SkubanaUserCredentials
	{
		public string AccessToken { get; private set; }

		public static SkubanaUserCredentials Blank = new SkubanaUserCredentials() {  AccessToken = string.Empty };

		private SkubanaUserCredentials()
		{
		}

		public SkubanaUserCredentials( string accessToken )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrEmpty();

			this.AccessToken = accessToken;
		}
	}
}