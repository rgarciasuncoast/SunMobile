using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class InsertPaymentResponse : BillPayResponseBase
    {
        [DataMember]
        public long PaymentId { get; set; }
    }
}