using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class SendOutOfBandVerificationRequest
	{
		[DataMember]
		public string TransactionId { get; set; }
		[DataMember]
		public string NotificationType { get; set; } // Email or Text
	}
}