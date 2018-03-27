using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.History
{
	[DataContract]
	public class AccountTransaction
	{
		[DataMember]
		public string TransactionType { get; set; }
		[DataMember]
		public string CheckNumber { get; set; }
		[DataMember]
		public DateTime PostingDate { get; set; }
		[DataMember]
		public DateTime TransactionDate { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public Decimal TransactionAmount { get; set; }
		[DataMember]
		public bool IsPending { get; set; }
		[DataMember]
		public string TraceNumber { get; set; }
		[DataMember]
		public int SequenceNumber { get; set; }
		[DataMember]
		public int SortOrder { get; set; }
		[DataMember]
		public int UserNumber { get; set; }
		[DataMember]
		public string ActionCode { get; set; }
		[DataMember]
		public string SourceCode { get; set; }
		[DataMember]
		public Decimal BalanceChange { get; set; }
		[DataMember]
		public string MerchantName { get; set; }
		[DataMember]
		public string CardNumber { get; set; }
		[DataMember]
		public string DisputedDate { get; set; }
		[DataMember]
		public string TransactionId { get; set; }
		[DataMember]
		public bool IsDisputable { get; set; }
	}
}