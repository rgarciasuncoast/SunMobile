using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransactionDisputeInformationResponse : MobileStatusResponse
	{
		[DataMember]
		public string DisputeInstructions { get; set; }
		[DataMember]
		public string AchDisputeInstructions { get; set; }
		[DataMember]
		public string AtmDisputeInstructions { get; set; }
		[DataMember]
		public string ReportLostStolenUrl { get; set; }
		[DataMember]
		public List<TransactionDisputeInfo> DisputeInfo { get; set; }
		[DataMember]
		public List<TransactionDisputeRule> DisputeRules { get; set; }
	}
}