using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class NextTransactionResponse : StatusResponse
	{
		[DataMember]
		public List<Transaction> Transactions { get; set; }
		[DataMember]
		public bool MoreData { get; set; }
		[DataMember]
		public string LastTransactionDate { get; set; }
		[DataMember]
		public string LastTransactionIdendtifier { get; set; }
	}
}