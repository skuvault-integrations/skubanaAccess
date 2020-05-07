using CuttingEdge.Conditions;
using SkubanaAccess.Configuration;
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
		///	Absolute URL
		/// </summary>
		public string Url
		{
			get
			{
				if ( !string.IsNullOrEmpty( this._absoluteUrl ) )
					return this._absoluteUrl;

				var absoluteUrl = $"{ this.Config.Environment.BaseUrl }{ this.RelativeUrl }";

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

		protected SkubanaCommand( SkubanaConfig config, string relativeUrl, Dictionary< string, string > requestParameters, object payload )
		{
			Condition.Requires( config, "config" ).IsNotNull();
			Condition.Requires( relativeUrl, "relativeUrl" ).IsNotNullOrEmpty();

			this.Config = config;
			this.RelativeUrl = relativeUrl;
			this.RequestParameters = requestParameters;
			this.Payload = payload;
		}

		protected SkubanaCommand( SkubanaConfig config, string relativeUrl ) : this( config, relativeUrl, null, null )
		{
		}

		protected SkubanaCommand( SkubanaConfig config, string relativeUrl, object payload ) : this( config, relativeUrl, null, payload )
		{
		}
	}
}