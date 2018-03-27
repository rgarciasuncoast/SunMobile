using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetPaymentsRequest
    {
        [DataMember]
        public long PaymentId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string PaymentStatus { get; set; }
        [DataMember]
        public long PayeeId { get; set; }
        [DataMember]
        public DateTime SearchBeginDate { get; set; }
        [DataMember]
        public DateTime SearchEndDate { get; set; }
    }
}