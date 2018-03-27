using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogPinChangeItem
    {
        [DataMember]
        [SensitiveData]
        public string PreviousPin { get; set; }

        [DataMember]
        [SensitiveData]
        public string NewPin { get; set; }
    }
}
