using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;
using SunBlock.DataTransferObjects.Collections;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;

namespace SunBlock.DataTransferObjects.Session
{
    [DataContract]
    public class SessionContext
    {
        [DataMember]
        public NameValueCollection SessionItems { get; set; }

        [DataMember]
        public Guid SessionId { get; set; }

        [DataMember]
        [SensitiveData]
        public string Username { get; set; }

        [DataMember]
        [SensitiveData]
        public string Password { get; set; }

        [DataMember]
        [SensitiveData]
        public string IpAddress { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public string CurrentState { get; set; }

        [DataMember]
        public DateTime Expiration { get; set; }

        [DataMember]
        public MemberInformation CurrentMember { get; set; }
    }
}
