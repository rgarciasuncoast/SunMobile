using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
	[DataContract]
	public class IsMemberEnrolledInBillPayResponse : BillPayResponseBase
	{
		[DataMember]
		public bool IsMemberEnrolledInBillPay { get; set; }
		[DataMember]
		public string BillPayEnrollmentAgreementText { get; set; }
	}
}