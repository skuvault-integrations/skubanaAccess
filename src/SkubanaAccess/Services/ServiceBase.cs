using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkubanaAccess.Configuration;
using SkubanaAccess.Exceptions;
using SkubanaAccess.Models.Commands;
using SkubanaAccess.Models.Infrastructure;
using SkubanaAccess.Shared;
using SkubanaAccess.Throttling;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkubanaAccess.Services
{
	public abstract class ServiceBase
	{
		protected SkubanaConfig Config { get; private set; }
		protected Throttler Throttler { get; private set; }
		protected HttpClient HttpClient { get; private set; }
		protected Func< string > _additionalLogInfo;

		/// <summary>
		///	Extra logging information
		/// </summary>
		public Func< string > AdditionalLogInfo
		{
			get { return this._additionalLogInfo ?? ( () => string.Empty ); }
			set => _additionalLogInfo = value;
		}

		public ServiceBase( SkubanaConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this.Config = config;
			this.Throttler = new Throttler( config.ThrottlingOptions.MaxRequestsPerTimeInterval, config.ThrottlingOptions.TimeIntervalInSec, config.ThrottlingOptions.MaxRetryAttempts );

			HttpClient = new HttpClient()
			{
				BaseAddress = new Uri( Config.Environment.BaseUrl ) 
			};
		}

		protected Task< T > PostAsync< T >( SkubanaCommand command, CancellationToken cancellationToken, Mark mark = null, [ CallerMemberName ] string methodName = "" )
		{
			return PostAsync< T >( command, cancellationToken, mark, false, null, methodName );
		}

		protected async Task< T > PostAsync< T >( SkubanaCommand command, CancellationToken cancellationToken, Mark mark = null, bool useBasicAuth = false, SkubanaAppCredentials appCredentials = null, [ CallerMemberName ] string methodName = "" )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			var responseContent = await this.ThrottleRequestAsync( command, mark, methodName, HttpMethod.Post, async ( token ) =>
			{
				this.SetAuthHeader( command, useBasicAuth, appCredentials );

				StringContent payload = null;
				if ( command.Payload != null )
				{
					payload = new StringContent( command.Payload.ToJson(), Encoding.UTF8, "application/json" );
					payload.Headers.ContentType = MediaTypeHeaderValue.Parse( "application/json" );
				}
				else
				{
					payload = new StringContent( string.Empty );
				}

				var httpResponse = await HttpClient.PostAsync( command.Url, payload ).ConfigureAwait( false );
				var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait( false );

				ThrowIfError( httpResponse, content );

				return content;
			}, cancellationToken ).ConfigureAwait( false );

			var response = JsonConvert.DeserializeObject< T >( responseContent );

			return response;
		}

		private void SetAuthHeader( SkubanaCommand command, bool useBasicAuth = false, SkubanaAppCredentials appCredentials = null )
		{
			this.HttpClient.DefaultRequestHeaders.Remove( "Authorization" );

			string headerValue = null;

			if ( useBasicAuth && appCredentials != null )
			{
				headerValue = $"Basic { Convert.ToBase64String( Encoding.UTF8.GetBytes( string.Concat( appCredentials.ApplicationKey, ":", appCredentials.ApplicationSecret ) ) ) }";
			}
			else
			{
				headerValue = $"Bearer { Convert.ToBase64String( Encoding.UTF8.GetBytes( this.Config.Credentials.AccessToken ) ) } ";
			}

			this.HttpClient.DefaultRequestHeaders.Add( "Authorization", headerValue );
		}

		protected void ThrowIfError( HttpResponseMessage response, string message )
		{
			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				throw new SkubanaUnauthorizedException( message );
			}
			else if ( !response.IsSuccessStatusCode )
			{
				throw new SkubanaException( message );
			}
		}

		private Task< T > ThrottleRequestAsync< T >( SkubanaCommand command, Mark mark, string methodName, HttpMethod methodType, Func< CancellationToken, Task< T > > processor, CancellationToken token )
		{
			return Throttler.ExecuteAsync( () =>
			{
				return new ActionPolicy( Config.NetworkOptions.RetryAttempts, Config.NetworkOptions.DelayBetweenFailedRequestsInSec, Config.NetworkOptions.DelayFailRequestRate )
					.ExecuteAsync( async () =>
					{
						using( var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) )
						{
							SkubanaLogger.LogStarted( this.CreateMethodCallInfo( command.Url, mark, methodType, payload: command.Payload, additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName ) );
							linkedTokenSource.CancelAfter( Config.NetworkOptions.RequestTimeoutMs );

							var result = await processor( linkedTokenSource.Token ).ConfigureAwait( false );

							SkubanaLogger.LogEnd( this.CreateMethodCallInfo( command.Url, mark, methodType, payload: command.Payload, responseBodyRaw: result.ToString(), additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName ) );

							return result;
						}
					}, 
					( exception, timeSpan, retryCount ) =>
					{
						string retryDetails = this.CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName );
						SkubanaLogger.LogTraceRetryStarted( timeSpan.Seconds, retryCount, retryDetails );
					},
					( ex ) => CreateMethodCallInfo( command.Url, mark, methodType, payload: command.Payload, additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName, errors: ex.Message ),
					SkubanaLogger.LogTraceException );
			} );
		}

		protected string CreateMethodCallInfo( string url = null, Mark mark = null, HttpMethod methodType = null, string errors = null, string responseBodyRaw = null, string additionalInfo = null, object payload = null, string libMethodName = null )
		{
			JObject responseBody = null;
			try
			{
				responseBody = JObject.Parse( responseBodyRaw );
			}
			catch { }

			return new CallInfo()
			{
				Mark = mark?.ToString() ?? "Unknown",
				Endpoint = url,
				Method = methodType?.ToString() ?? "Uknown",
				Body = payload,
				LibMethodName = libMethodName,
				AdditionalInfo = additionalInfo,
				Response = (object)responseBody ?? responseBodyRaw,
				Errors = errors
			}.ToJson();
		}
	}
}