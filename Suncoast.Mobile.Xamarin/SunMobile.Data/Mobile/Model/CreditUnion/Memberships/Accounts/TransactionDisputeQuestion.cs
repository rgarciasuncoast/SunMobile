using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransactionDisputeQuestion
	{
		[DataMember]
		public string Question { get; set; }
		[DataMember]
		public bool IsAnswerRequired { get; set; }
		[DataMember]
		public string OnBaseFormField { get; set; }
		[DataMember]
		public string AnswerType { get; set; }   // string, bool, datetime, decimal, int, list
		[DataMember]
		public string Hint { get; set; }
		[DataMember]
		public int MinLength { get; set; }
		[DataMember]
		public int MaxLength { get; set; }
		[DataMember]
		public int VisibleDependencyQuestion { get; set; }
		[DataMember]
		public string VisibleDependencyAnswer { get; set; }
		[DataMember]
		public bool HideIfNegativeTransactionAmount { get; set; }
		[DataMember]
		public bool ShowAdditionalTransactionsIfAnswerTrue { get; set; }
		[DataMember]
		public List<string> DefaultValues { get; set; }
	}
}