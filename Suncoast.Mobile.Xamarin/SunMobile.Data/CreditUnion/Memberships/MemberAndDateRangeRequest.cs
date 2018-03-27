using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships
{
    [DataContract]
    public class MemberAndDateRangeRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public DateTime BeginDateUtc { get; set; }
        [DataMember]
        public DateTime EndDateUtc { get; set; }
    }
}
