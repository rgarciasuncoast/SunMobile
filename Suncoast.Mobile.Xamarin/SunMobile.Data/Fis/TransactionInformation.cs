using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.History;

namespace SunBlock.DataTransferObjects.Fis
{
    [DataContract]
    public class TransactionInformation
    {
        [DataMember]
        public string CreditCardNumber { get; set; }
        [DataMember]
        public List<CreditCardTransaction> Transactions { get; set; }
        [DataMember]
        public DateTime BeginDateEstUtc { get; set; }
        [DataMember]
        public DateTime EndDateEstUtc { get; set; }

    }
}
