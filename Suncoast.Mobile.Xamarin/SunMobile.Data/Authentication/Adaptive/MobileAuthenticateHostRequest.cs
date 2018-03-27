using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
	[DataContract]
	public class MobileAuthenticateHostRequest : AuthenticateHostRequest
	{
		[DataMember]
		public string Version { get; set; }
		[DataMember]
		public string DeviceId { get; set; }
		[DataMember]
		public PayloadMessage Payload { get; set; }
	}
}