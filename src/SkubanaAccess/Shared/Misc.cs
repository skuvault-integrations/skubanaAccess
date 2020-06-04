using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SkubanaAccess.Shared
{
	public static class Misc
	{
		public static string ToJson( this object source )
		{
			try
			{
				if (source == null)
					return "{}";
				else
				{
					var serialized = JsonConvert.SerializeObject( source, new IsoDateTimeConverter() );
					return serialized;
				}
			}
			catch( Exception )
			{
				return "{}";
			}
		}

		public static List< Dictionary< K, V > > SplitToChunks< K, V >( this Dictionary< K, V > source, int chunkSize )
		{
			var i = 0;
			var chunks = new List< Dictionary< K, V > >();
			
			while( i < source.Count() )
			{
				var temp = source.Skip( i ).Take( chunkSize ).ToDictionary( x => x.Key, x => x.Value );
				chunks.Add( temp );
				i += chunkSize;
			}
			return chunks;
		}

		public static List< List< T > > SplitToChunks< T >( this IEnumerable< T > source, int chunkSize )
		{
			var i = 0;
			var chunks = new List< List< T > >();
			
			while( i < source.Count() )
			{
				var temp = source.Skip( i ).Take( chunkSize ).ToList();
				chunks.Add( temp );
				i += chunkSize;
			}
			return chunks;
		}

		public static DateTime ConvertStrToDateTime( this string str )
		{
			if ( string.IsNullOrEmpty( str ) )
				return DateTime.MinValue;

			if ( DateTime.TryParseExact( str, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result ) )
				return result;
			
			return DateTime.MinValue;
		}

		public static string ConvertDateTimeToStr( this DateTime date )
		{
			return date.ToString( "yyyy-MM-ddTHH:mm:ssZ" );
		}
	}
}