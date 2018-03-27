using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class OnlineSessionData
    {
        [Queryable]
        [SensitiveData]
        [DataMember]
        public string MemberId { get; set; }

        [Queryable]
        [DataMember]
        public DateTime SessionStartDate { get; set; }

        [Queryable]
        [DataMember]
        public Guid SessionId { get; set; }

        [Queryable]
        [DataMember]
        public string IpAddress { get; set; }
    }
}
