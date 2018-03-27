using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetRecurringPaymentFrequenciesResponse : BillPayResponseBase
    {
        [DataMember]
        public List<RecurringPaymentFrequency> RecurringPaymentFrequencies { get; set; }
    }
}