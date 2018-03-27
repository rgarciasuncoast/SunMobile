using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class HighRiskToken
    {
        [Queryable]
        [DataMember]
        public Guid Id { get; set; }
        [Queryable]
        [SensitiveData]
        [DataMember]
        public string MemberId { get; set; }
        [Queryable]
        [SensitiveData]
        [DataMember]
        public string Token { get; set; }
        [Queryable]
        [DataMember]
        public DateTime ExpireTimeUtc { get; set; }
        [Queryable]
        [DataMember]
        public DateTime EntryTimeUtc { get; set; }
        [Queryable]
        [SensitiveData]
        [DataMember]
        public string IpAddress { get; set; }
        [Queryable]
        [DataMember]
        public bool Verified { get; set; }
    }
}
