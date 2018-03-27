using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay.V2
{
	[DataContract]
	public class DoesPayeeHavePendingPaymentsRequest
	{
		[DataMember]
		public int MemberId { get; set; }
		[DataMember]
		public long MemberPayeeId { get; set; }
	}
}