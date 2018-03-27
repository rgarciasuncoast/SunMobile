using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class LoanAccount : CreditLineAccount
    {
        [DataMember]
        public decimal EscrowBalance { get; set; }

        [DataMember]
        public decimal PayoffAmount { get; set; }

        [DataMember]
        public decimal AvailableCashAdvance { get; set; }
    }
}