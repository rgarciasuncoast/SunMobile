using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetPendingPaymentStatusResponse : BillPayResponseBase
    {
        [DataMember]
        public int PendingPaymentsCount { get; set; }
        [DataMember]
        public decimal PendingPaymentsTotal { get; set; }
        [DataMember]
        public DateTime NextPaymentDate { get; set; }
    }
}
