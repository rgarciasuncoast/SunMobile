using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class SessionRequest
    {
        [DataMember]
        public Guid SessionId { get; set; }

        [DataMember]
        public int MemberId { get; set; }
    }
}
