using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Member
{
    [DataContract]
    public class MemberInformation
    {
        [DataMember]
        public Int32 MemberId { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string FaxNumber { get; set; }
        [DataMember]
        public string CellNumber { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public DateTime DateOfBirth { get; set; }
        [DataMember]
        public string TaxId { get; set; }
        [DataMember]
        public DateTime LastOnlineAccessTime { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string HomePhone { get; set; }
        [DataMember]
        public string WorkPhone { get; set; }
        [DataMember]
        public bool RestrictOnlineAccess { get; set; }
        [DataMember]
        public DateTime OpenedDate { get; set; }
        [DataMember]
        public int IncomingDailyCrossAccountTransfers { get; set; }
        [DataMember]
        public decimal DailyCrossAccountAmount { get; set; }
        [DataMember]
        public TransactionDisputeRestrictions TransactionDisputeRestrictions { get; set; }
    }
}
