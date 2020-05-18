using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
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
	}
}