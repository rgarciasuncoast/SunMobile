using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class Account
    {
        /// <summary>
        /// Represents Account Types:
        /// Shares,
        /// Loans,
        /// CreditCards,
        /// Mortgages,
        /// ShareCertificates,
        /// IRAs,
        /// ShareDrafts,
        /// Certificates,
        /// MoneyMarkets,
        /// Clubs
        /// </summary>
        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        public string Suffix { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string HostDescription { get; set; }

        [DataMember]
        public decimal Available { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public bool IsAllowedForTransferSource { get; set; }

        [DataMember]
        public bool IsAllowedForTransferTarget { get; set; }

        [DataMember]
        public bool RequestChecks { get; set; }

        [DataMember]
        public bool IsCheckingAccount { get; set; }

        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public string OwnershipType { get; set; }

        [DataMember]
        public string OwnerName { get; set; }

        [DataMember]
        public int NameType { get; set; }
    
        [DataMember]
        public int HostType { get; set; }
    
}
}