using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogFlagItem
    {
        [DataMember]
        [SensitiveData]
        public string FlagNumber { get; set; }

        [DataMember]        
        public bool isClearFlag { get; set; }

        [DataMember]
        public bool isInvalid { get; set; }
    }
}
