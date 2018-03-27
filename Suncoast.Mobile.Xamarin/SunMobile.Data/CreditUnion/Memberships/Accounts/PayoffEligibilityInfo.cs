using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class PayoffEligibilityInfo
    {
        [DataMember]
        public bool IsPayoffEligible { get; set; }
        [DataMember]
        public List<string> ValidDatesForPayoff { get; set; }
    }
}