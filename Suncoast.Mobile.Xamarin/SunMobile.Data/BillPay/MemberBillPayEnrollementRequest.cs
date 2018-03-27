using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class MemberBillPayEnrollementRequest
    {
        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public bool IsUnEnroll { get; set; }
    }
}