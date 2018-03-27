using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class ShareAccount : Account
    {
        [DataMember]
        public decimal DividendRate { get; set; }

        [DataMember]
        public int HostCode { get; set; }

        [DataMember]
        public decimal YearToDateDividends { get; set; }

        [DataMember]
        public decimal PreviousYearToDateDividends { get; set; }

        [DataMember]
        public DateTime MaturityDate { get; set; }
    }
}
