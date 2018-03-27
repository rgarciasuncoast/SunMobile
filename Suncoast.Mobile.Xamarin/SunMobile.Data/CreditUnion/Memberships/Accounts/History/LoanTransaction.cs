using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.History
{
    [DataContract]
    public class LoanTransaction : BalanceTransaction
    {
        [DataMember]
        public decimal Interest { get; set; }
        [DataMember]
        public decimal Principal { get; set; }
        [DataMember]
        public decimal Escrow { get; set; }
    }
}
