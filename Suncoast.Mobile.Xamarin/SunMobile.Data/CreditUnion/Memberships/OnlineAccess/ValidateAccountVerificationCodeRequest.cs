using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class ValidateAccountVerificationCodeRequest
	{
		[DataMember]
		public string NotificationType { get; set; }
		[DataMember]
		public string ValidationCode { get; set; }
		[DataMember]
		public string LastEight { get; set; }
		[DataMember]
		public string DeviceId { get; set; }
	}
}