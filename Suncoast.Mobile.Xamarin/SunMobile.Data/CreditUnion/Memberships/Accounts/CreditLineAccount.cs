using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class CreditLineAccount : Account
    {
        [DataMember]
        public decimal InterestRate { get; set; }
        [DataMember]
        public decimal PaymentAmount { get; set; }
        [DataMember]
        public DateTime PaymentDueDate { get; set; }

    }
}
