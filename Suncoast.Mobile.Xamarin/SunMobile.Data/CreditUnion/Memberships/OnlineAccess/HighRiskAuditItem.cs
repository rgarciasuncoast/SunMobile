using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class HighRiskAuditItem
    {
        [Queryable]
        [DataMember]
        public Guid Id { get; set; }
        [Queryable]
        [SensitiveData]
        [DataMember]
        public string MemberId { get; set; }
        [Queryable]
        [DataMember]
        public string AuditType { get; set; }
        [Queryable]
        [DataMember]
        public string IpAddress { get; set; }
        [Queryable]
        [DataMember]
        public DateTime EntryTimeUtc { get; set; }
        [Queryable]
        [DataMember]
        public string Details { get; set; }
    }
}
