using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.ODBC
{
    [DataContract]
    public class CreditCardDetails
    {
        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public decimal MinimumDue { get; set; }

        [DataMember]
        public decimal Available { get; set; }

        [DataMember]
        public decimal CreditLimit { get; set; }

        [DataMember]
        public DateTime DateLastUpdated { get; set; } 
    }
}
