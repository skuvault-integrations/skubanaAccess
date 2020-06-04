using CuttingEdge.Conditions;

namespace SkubanaAccess.Configuration
{
	public class SkubanaConfig
	{
		public SkubanaEnvironment Environment { get; set; }
		public SkubanaUserCredentials Credentials { get; private set; }

		public ThrottlingOptions ThrottlingOptions { get; private set; }
		public NetworkOptions NetworkOptions { get; private set; }

		public int RetrieveProductsStocksTotalPageSize { get; set; }
		public int RetrieveProductsPageSize { get; set; }
		public int RetrieveOrdersPageSize { get; set; }

		public int UpdateProductStockBatchSize { get; set; }
		public int CreateProductStockBatchSize { get; set; }
		public int RetrieveProductsBySkusBatchSize { get; set; }

		public SkubanaConfig( SkubanaEnvironment environment, SkubanaUserCredentials credentials, ThrottlingOptions throttlingOptions, NetworkOptions networkOptions )
		{
			Condition.Requires( environment, "environment" ).IsNotNull();
			Condition.Requires( credentials, "credentials" ).IsNotNull();
			Condition.Requires( throttlingOptions, "throttlingOptions" ).IsNotNull();
			Condition.Requires( networkOptions, "networkOptions" ).IsNotNull();

			this.Environment = environment;
			this.Credentials = credentials;
			this.ThrottlingOptions = throttlingOptions;
			this.NetworkOptions = networkOptions;

			this.RetrieveProductsStocksTotalPageSize = 200;

			this.UpdateProductStockBatchSize = 200;
			this.CreateProductStockBatchSize = 200;
			this.RetrieveProductsBySkusBatchSize = 10;
			this.RetrieveProductsPageSize = 20;
			this.RetrieveOrdersPageSize = 100;
		}

		public SkubanaConfig( SkubanaEnvironment environment, SkubanaUserCredentials credentials )
			: this( environment, credentials, ThrottlingOptions.SkubanaDefaultThrottlingOptions, NetworkOptions.SkubanaDefaultNetworkOptions )
		{
		}

		public SkubanaConfig( SkubanaEnvironment environment )
			: this( environment, SkubanaUserCredentials.Blank, ThrottlingOptions.SkubanaDefaultThrottlingOptions, NetworkOptions.SkubanaDefaultNetworkOptions )
		{
		}
	}

	public class SkubanaEnvironment
	{
		public static SkubanaEnvironment Sandbox = new SkubanaEnvironment( SkubanaEnvironmentEnum.Sandbox, "https://demo.skubana.com", "https://api.demo.skubana.com" );
		public static SkubanaEnvironment Production = new SkubanaEnvironment( SkubanaEnvironmentEnum.Production, "https://app.skubana.com", "https://api.skubana.com" );

		public SkubanaEnvironmentEnum Type { get; private set; }
		public string BaseAuthUrl { get; private set; }
		public string BaseApiUrl { get; private set; }

		private SkubanaEnvironment( SkubanaEnvironmentEnum type, string baseAuthUrl, string baseApiUrl )
		{
			this.Type = type;
			this.BaseAuthUrl = baseAuthUrl;
			this.BaseApiUrl = baseApiUrl;
		}
	}

	public enum SkubanaEnvironmentEnum
	{
		Sandbox,
		Production
	}

	public class ThrottlingOptions
	{
		public int MaxRequestsPerTimeInterval { get; private set; }
		public int TimeIntervalInSec { get; private set; }
		public int MaxRetryAttempts { get; private set; }

		public ThrottlingOptions( int maxRequests, int timeIntervalInSec, int maxRetryAttempts )
		{
			Condition.Requires( maxRequests, "maxRequests" ).IsGreaterOrEqual( 1 );
			Condition.Requires( timeIntervalInSec, "timeIntervalInSec" ).IsGreaterOrEqual( 1 );
			Condition.Requires( maxRetryAttempts, "maxRetryAttempts" ).IsGreaterOrEqual( 0 );

			this.MaxRequestsPerTimeInterval = maxRequests;
			this.TimeIntervalInSec = timeIntervalInSec;
			this.MaxRetryAttempts = maxRetryAttempts;
		}

		public static ThrottlingOptions SkubanaDefaultThrottlingOptions
		{
			get
			{
				return new ThrottlingOptions( 5, 1, 10 );
			}
		}
	}

	public class NetworkOptions
	{
		public int RequestTimeoutMs { get; private set; }
		public int RetryAttempts { get; private set; }
		public int DelayBetweenFailedRequestsInSec { get; private set; }
		public int DelayFailRequestRate { get; private set; }

		public NetworkOptions( int requestTimeoutMs, int retryAttempts, int delayBetweenFailedRequestsInSec, int delayFaileRequestRate )
		{
			Condition.Requires( requestTimeoutMs, "requestTimeoutMs" ).IsGreaterThan( 0 );
			Condition.Requires( retryAttempts, "retryAttempts" ).IsGreaterOrEqual( 0 );
			Condition.Requires( delayBetweenFailedRequestsInSec, "delayBetweenFailedRequestsInSec" ).IsGreaterOrEqual( 0 );
			Condition.Requires( delayFaileRequestRate, "delayFaileRequestRate" ).IsGreaterOrEqual( 0 );

			this.RequestTimeoutMs = requestTimeoutMs;
			this.RetryAttempts = retryAttempts;
			this.DelayBetweenFailedRequestsInSec = delayBetweenFailedRequestsInSec;
			this.DelayFailRequestRate = delayFaileRequestRate;
		}

		public static NetworkOptions SkubanaDefaultNetworkOptions
		{
			get
			{
				return new NetworkOptions( 5 * 60 * 1000, 10, 5, 20 );
			}
		}
	}
}
