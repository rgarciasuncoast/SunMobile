using System;

namespace SunMobile.Shared.Data
{
	public class Alert
	{
		public string Id { get; set; }
		public DateTime ReceivedDate { get; set; }
		public string Message { get; set; }	
		public Boolean IsRead { get; set; }
	}
}