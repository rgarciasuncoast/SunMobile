using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	public class EStatementOptInViewedResponse : StatusResponse
	{
		[DataMember]
		public bool EStatementOptInViewed { get; set; }
		[DataMember]
		public bool EStatementsEnrolled { get; set; }
		[DataMember]
		public string AgreementText { get; set; }
	}
}