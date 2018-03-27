using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class OutOfBandChallengeRequiredRequest
	{
		[DataMember]
		public string TransactionType { get; set; }
		[DataMember]
		public PayloadMessage Payload { get; set; }
	}
}