using System;

namespace SkubanaAccess.Exceptions
{
	public class SkubanaException : Exception
	{
		public SkubanaException( string message, Exception innerException ) : base( message, innerException ) 
		{
		}
		
		public SkubanaException( string message ) : this ( message, null ) { }
	}
}