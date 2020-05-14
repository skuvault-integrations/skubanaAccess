using System.Collections.Generic;

namespace SkubanaAccess.Models.Response
{
	public class SkubanaResponse< T >
	{
		public IEnumerable< T > Results { get; set; }
		public IEnumerable< SkubanaResponseError > Errors { get; set; }
	}

	public class SkubanaResponseError
	{
		public string Message { get; set; }
		public string ErrorCode { get; set; }
		public string Identifier { get; set; }
		public int Index { get; set; }
	}
}