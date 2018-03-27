using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class RecurringPaymentFrequency
    {
        [DataMember]
        public int ApplicationId { get; set; }
        [DataMember]
        public string ApplicationUserName { get; set; }
        [DataMember]
        public Guid EventExceptionGuid { get; set; }
        [DataMember]
        public bool EventForceLog { get; set; }
        [DataMember]
        public int FrequencyId { get; set; }
        [DataMember]
        public string FrequencyName { get; set; }
    }
}