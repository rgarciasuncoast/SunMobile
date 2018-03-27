using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class IsOnlineDisclosureAcceptedResponse : StatusResponse
	{
		[DataMember]
		public bool IsAccepted { get; set; }
		[DataMember]
		public string OnlineBankingAgreementText { get; set; }
	}
}