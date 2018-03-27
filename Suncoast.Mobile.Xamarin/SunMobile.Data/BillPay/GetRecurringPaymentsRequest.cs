using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetRecurringPaymentsRequest
    {
        [DataMember]
        public long RecPaymentId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public long PayeeId { get; set; }
    }
}