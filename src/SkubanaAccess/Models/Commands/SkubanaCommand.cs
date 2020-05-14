using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
using SkubanaAccess.Throttling;
using System.Collections.Generic;
using System.Linq;

namespace SkubanaAccess.Models.Commands
{
	public abstract class SkubanaCommand
	{
		/// <summary>
		///	HTTP body content
		/// </summary>
		public object Payload { get; set; }

		/// <summary>
		///	Custom throttler for request
		/// </summary>
		public Throttler Throttler { get; set; }

		/// <summary>
		///	Absolute URL
		/// </summary>
		public string Url
		{
			get
			{
				if ( !string.IsNullOrEmpty( this._absoluteUrl ) )
					return this._absoluteUrl;

				var absoluteUrl = $"{ ( this._isApiCommand ? this.Config.Environment.BaseApiUrl : this.Config.Environment.BaseUrl ) }{ this.RelativeUrl }";

				if ( this.RequestParameters != null && this.RequestParameters.Any() )
				{
					absoluteUrl += string.Concat( "?", string.Join( "&", this.RequestParameters.Select( p => string.Concat( p.Key, "=", p.Value ) ) ) );
				}

				this._absoluteUrl = absoluteUrl;
				return this._absoluteUrl;
			}
		}

		protected SkubanaConfig Config { get; private set; }
		protected string RelativeUrl { get; private set; }
		protected Dictionary< string, string > RequestParameters { get; set; }
		
		private string _absoluteUrl;
		private bool _isApiCommand;

		protected SkubanaCommand( SkubanaConfig config, string relativeUrl, Dictionary< string, string > requestParameters, object payload, bool isApiCommand = true )
		{
			Condition.Requires( config, "config" ).IsNotNull();
			Condition.Requires( relativeUrl, "relativeUrl" ).IsNotNullOrEmpty();

			this.Config = config;
			this.RelativeUrl = relativeUrl;
			this.RequestParameters = requestParameters;
			this.Payload = payload;
			this._isApiCommand = isApiCommand;
		}

		protected SkubanaCommand( SkubanaConfig config, string relativeUrl ) : this( config, relativeUrl, null, null )
		{
		}

		protected SkubanaCommand( SkubanaConfig config, string relativeUrl, object payload ) : this( config, relativeUrl, null, payload )
		{
		}
	}

	public class SkubanaEndpoint
	{
		public const string GetAccessTokenUrl = "/oauth/token";
		public const string GetProductsStocksTotalUrl = "/v1/productstocks/total";
		public const string AdjustProductStockUrl = "/v1.1/inventory/adjust";
		public const string CreateProductStockUrl = "/v1.1/inventory";
		public const string RetrieveProductsUrl = "/v1.1/products";
		public const string ListWarehousesUrl = "/v1/warehouses";
	}
}