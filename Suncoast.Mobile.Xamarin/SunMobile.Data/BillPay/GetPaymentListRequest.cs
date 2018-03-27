using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetPaymentListRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public bool IsPending { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
    }
}