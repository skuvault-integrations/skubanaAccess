using System;

namespace SkubanaAccess.Exceptions
{
	public class SkubanaNetworkException : SkubanaException
	{
		public SkubanaNetworkException( string message, Exception innerException ) : base( message, innerException ) { }
		public SkubanaNetworkException( string message ) : base( message, null ) { }
	}

	public class SkubanaUnauthorizedException : SkubanaException
	{
		public SkubanaUnauthorizedException( string message ) : base( message) { }
	}

	public class SkubanaRequestQuotaExceeded : SkubanaNetworkException
	{
		public SkubanaRequestQuotaExceeded( string message ) : base( message ) { }
	}
}