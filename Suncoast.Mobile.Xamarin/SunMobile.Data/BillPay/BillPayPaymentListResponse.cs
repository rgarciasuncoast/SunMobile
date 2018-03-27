using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class BillPayPaymentListResponse : BillPayResponseBase {
        [DataMember]
        public List<Payment> Payments { get; set; }
    }
}