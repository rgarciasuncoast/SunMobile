using System;
using System.Net;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class IpAndDateRangeRequest
    {
        [DataMember]
        public string IpAddress { get; set; }
        [DataMember]
        public DateTime BeginDateUtc { get; set; }
        [DataMember]
        public DateTime EndDateUtc { get; set; }
    }
}
