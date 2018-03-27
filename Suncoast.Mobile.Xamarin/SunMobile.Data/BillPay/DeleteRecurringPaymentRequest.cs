using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class DeleteRecurringPaymentRequest
    {
        [DataMember]
        public long RecPaymentId { get; set; }
    }
}