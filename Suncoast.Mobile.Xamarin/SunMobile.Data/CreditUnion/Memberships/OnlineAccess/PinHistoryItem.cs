using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class PinHistoryItem
    {
        [DataMember]
        [SensitiveData]
        public string Pin { get; set; }
        [DataMember]
        public DateTime ChangedToDateUtc { get; set; }
        [DataMember]
        public DateTime ChangedFromDateUtc { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}
