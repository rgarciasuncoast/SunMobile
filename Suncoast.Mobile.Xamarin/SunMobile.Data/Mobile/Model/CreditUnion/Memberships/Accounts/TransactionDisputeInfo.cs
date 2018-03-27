using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransactionDisputeInfo
	{
		[DataMember]
		public string DisputeReason { get; set; }
		[DataMember]
		public bool CanSelectMultipleTransactions { get; set; }
		[DataMember]
		public bool CanSubmitDocuments { get; set; }
		[DataMember]
		public string DisputeAgreement { get; set; }
		[DataMember]
		public List<TransactionDisputeQuestion> DisputeQuestions { get; set; }
	}
}