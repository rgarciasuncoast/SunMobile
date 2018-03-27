using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay.V2
{
	[DataContract]
	public class GetPayeesRequest
	{
		[DataMember]
		public int MemberId { get; set; }
	}
}