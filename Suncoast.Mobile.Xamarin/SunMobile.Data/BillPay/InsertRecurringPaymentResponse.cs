using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class InsertRecurringPaymentResponse : BillPayResponseBase
    {
        [DataMember]
        public long RecPaymentId { get; set; }
    }
}