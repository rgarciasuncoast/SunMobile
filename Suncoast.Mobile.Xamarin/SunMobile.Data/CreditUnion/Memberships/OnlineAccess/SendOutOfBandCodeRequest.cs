using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class SendOutOfBandCodeRequest
	{
		[DataMember]
		public string TransactionType { get; set; }
		[DataMember]
		public PayloadMessage Payload { get; set; }
		[DataMember]
		public string OutOfBandMessageType { get; set; }
	}
}