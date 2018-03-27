using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.History
{
    [DataContract]
    public class BalanceTransaction : AccountTransaction
    {
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public string TransactionCode { get; set; }
    }
}
