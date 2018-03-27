using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class IsMemberEnrolledInBillPayRequest
    {
        [DataMember]
        public int MemberId { get; set; }
    }
}