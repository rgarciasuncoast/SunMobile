using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class SendAccountVerificationCodeRequest
	{
		[DataMember]
		public string NotificationType { get; set; }
		[DataMember]
		public string Destination { get; set; }
		[DataMember]
		public string DeviceId { get; set; }
	}
}