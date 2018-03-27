using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class AccountList
    {
        [DataMember]
        public List<ShareAccount> Shares { get; set; }

        [DataMember]
        public List<LoanAccount> Loans { get; set; }

        [DataMember]
        public List<CreditCardAccount> CreditCards { get; set; }
    }
}
