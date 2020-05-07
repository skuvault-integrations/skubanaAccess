using Netco.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SkubanaAccess.Shared
{
	public class SkubanaLogger
	{
		private static readonly string _versionInfo;
		private const string integrationName = "Skubana";
		private const int MaxLogLineSize = 0xA00000; //10mb

		static SkubanaLogger()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			_versionInfo = FileVersionInfo.GetVersionInfo( assembly.Location ).FileVersion;
		}

		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "SkubanaLogger" );
		}

		public static void LogTraceException( Exception exception )
		{
			Log().Error( exception, "[{channel}] [ver:{version}] An exception occured. ", integrationName, _versionInfo );
		}

		public static void LogTraceStarted( string info )
		{
			TraceLog( "Trace Start call", info );
		}

		public static void LogTraceEnd( string info )
		{
			TraceLog( "Trace End call", info );
		}

		public static void LogStarted( string info )
		{
			TraceLog( "Start call", info );
		}

		public static void LogEnd( string info )
		{
			TraceLog( "End call", info );
		}

		public static void LogTrace( Exception ex, string info )
		{
			TraceLog( "Trace info", info );
		}

		public static void LogTrace( string info )
		{
			TraceLog( "Trace info", info );
		}

		public static void LogTraceRetryStarted( int delaySeconds, int attempt, string info )
		{
			info = String.Format( "{0}, Delay: {0}s, Attempt: {1} ", info, delaySeconds, attempt );
			TraceLog( "Trace info", info );
		}

		public static void LogTraceRetryEnd( string info )
		{
			TraceLog( "TraceRetryEnd info", info );
		}

		private static void TraceLog( string type, string info )
		{
			if( info.Length < MaxLogLineSize )
			{
				Log().Trace( "[{type}] [{channel}] [ver:{version}] {info}", type, integrationName, _versionInfo, info );
				return;
			}

			var pageNumber = 1;
			var pageId = Guid.NewGuid();
			foreach( var page in SplitString( info, MaxLogLineSize ) )
			{
				Log().Trace( "[{type}] [{channel}] [ver:{version}] page:{page} pageId:{pageId} {info}", type, integrationName, _versionInfo, pageNumber++, pageId, page );
			}
		}

		private static IEnumerable< string > SplitString( string str, int chunkSize )
		{
			return Enumerable.Range( 0, str.Length / chunkSize )
				.Select( i => str.Substring( i * chunkSize, chunkSize ) );
		}
	}
}