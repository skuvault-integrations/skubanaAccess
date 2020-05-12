namespace SkubanaAccess.Models.Infrastructure
{
	public class CallInfo
	{
		public string Mark { get; set; }
		public string LibMethodName { get; set; }
		public string Endpoint { get; set; }
		public string Method { get; set; }
		public object Body { get; set; }
		public string AdditionalInfo { get; set; }
		public object Response { get; set; }
		public string Errors { get; set; }
	}
}