using CsvHelper;
using CsvHelper.Configuration;
using SkubanaAccess.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SkubanaAccessTests
{
	public abstract class BaseTest
	{
		protected SkubanaConfig Config { get; private set; }
		protected SkubanaAppCredentials AppCredentials { get; private set; }

		public BaseTest()
		{
			var testCredentials = this.LoadTestSettings< TestCredentials >( @"\..\..\credentials.csv" );
			var env = (SkubanaEnvironmentEnum)Enum.Parse( typeof( SkubanaEnvironmentEnum ), testCredentials.Environment );

			this.Config = new SkubanaConfig( env == SkubanaEnvironmentEnum.Production ? SkubanaEnvironment.Production : SkubanaEnvironment.Sandbox,
									new SkubanaUserCredentials( testCredentials.AccessToken ) );
			this.AppCredentials = new SkubanaAppCredentials( testCredentials.ApplicationKey, testCredentials.ApplicationSecret, testCredentials.RedirectUrl,
												testCredentials.Scopes.Split( ' ' ) );
		}

		protected T LoadTestSettings< T >( string filePath )
		{
			string basePath = new Uri( Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase ) ).LocalPath;

			using( var streamReader = new StreamReader( basePath + filePath ) )
			{
				var csvConfig = new Configuration()
				{
					Delimiter = ",", 
					HasHeaderRecord = true
				};

				using( var csvReader = new CsvReader( streamReader, csvConfig ) )
				{
					var credentials = csvReader.GetRecords< T >();

					return credentials.FirstOrDefault();
				}
			}
		}
	}
}