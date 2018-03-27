using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetPaymentRecentActivityRequest
    {
        [DataMember]
        public long PaymentId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string PaymentStatus { get; set; }
        [DataMember]
        public DateTime SearchBeginDate { get; set; }
        [DataMember]
        public DateTime SearchEndDate { get; set; }
    }
}