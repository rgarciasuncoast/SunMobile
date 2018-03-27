using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public string AccountCategory { get; set; }
        [DataMember]
        public string AccountType { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal AvailableBalance { get; set; }
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public bool IsAllowedForTransferSource { get; set; }
        [DataMember]
        public bool IsAllowedForTransferTarget { get; set; }
        [DataMember]
        public bool IsAllowedForBillPay { get; set; }
        [DataMember]
        public bool IsCheckingAccount { get; set; }
        [DataMember]
        public bool IsChecklessChecking { get; set; }
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
        [DataMember]
        public decimal PaymentAmount { get; set; }
        [DataMember]
        public DateTime PaymentDueDate { get; set; }
        [DataMember]
        public string MicrAccountNumber { get; set; }
        [DataMember]
        public decimal InterestRate { get; set; }
        [DataMember]
        public PayoffEligibilityInfo PayoffEligibility { get; set; }
    }
}