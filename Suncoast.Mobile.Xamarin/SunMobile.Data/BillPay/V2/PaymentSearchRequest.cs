using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay.V2
{
    [DataContract]
    public class PaymentSearchRequest {
        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public long MemberPayeeId { get; set; }
    }
}