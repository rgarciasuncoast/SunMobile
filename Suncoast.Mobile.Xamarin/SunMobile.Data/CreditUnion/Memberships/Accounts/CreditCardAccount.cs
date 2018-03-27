using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class CreditCardAccount : CreditLineAccount
    {
        [DataMember]
        public decimal CreditLimit { get; set; }

        [DataMember]
        public DateTime LastPaymentDate { get; set; }

        [DataMember]
        public decimal LastPaymentAmount { get; set; }

        [DataMember]
        public decimal LastStatementBalance { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public DateTime OpenedDate { get; set; }

        [DataMember]
        public DateTime LastBlockedDate { get; set; }

        [DataMember]
        public string PreviousAccountNumber { get; set; }

        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        public decimal LateCharges { get; set; }

        [DataMember]
        public bool Delinquent { get; set; }
    }
}
