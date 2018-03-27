using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Fis
{
    [DataContract]
    public class GeneralCardHolderInformation
    {
        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public decimal MinimumDue { get; set; }

        [DataMember]
        public decimal AvailableCredit { get; set; }

        [DataMember]
        public decimal CreditLimit { get; set; }

        [DataMember]
        public string CardStatus { get; set; }
 
        [DataMember]
        public DateTime NextPaymentDate { get; set; }

        [DataMember]
        public DateTime LastPaymentDate { get; set; }

        [DataMember]
        public decimal LastPaymentAmount { get; set; }

        [DataMember]
        public decimal LastStatementBalance { get; set; }

        [DataMember]
        public decimal AmountPastDue { get; set; }

        [DataMember]
        public decimal PayoffAmount { get; set; }

        [DataMember]
        public DateTime OpenedDate { get; set; }

        [DataMember]
        public DateTime LastBlockedDate { get; set; }

        [DataMember]
        public string PreviousAccountNumber { get; set; }

        [DataMember]
        public bool IsValid { get; set; }
    }
}
