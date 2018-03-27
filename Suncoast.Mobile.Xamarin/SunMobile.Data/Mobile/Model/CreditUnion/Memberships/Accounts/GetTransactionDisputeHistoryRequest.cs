using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class GetTransactionDisputeHistoryRequest
	{
		[DataMember]
		public int MemberId { get; set; }

		[DataMember]
		public string BeginDate { get; set; }   //yyyyMMdd

		[DataMember]
		public string EndDate { get; set; } //yyyyMMdd

	}
}