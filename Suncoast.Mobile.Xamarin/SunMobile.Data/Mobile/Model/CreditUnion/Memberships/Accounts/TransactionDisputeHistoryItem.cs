using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransactionDisputeHistoryItem
	{
		[DataMember]
		public int MemberId { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public decimal Amount { get; set; }

		[DataMember]
		public string DateOfTransaction { get; set; } //yyyyMMdd

		[DataMember]
		public string DateDisputed { get; set; } //yyyyMMdd

		[DataMember]
		public string TransactionId { get; set; }
	}
}