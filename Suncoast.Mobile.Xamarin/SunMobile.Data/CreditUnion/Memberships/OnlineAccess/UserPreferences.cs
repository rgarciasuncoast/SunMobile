using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class UserPreference
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public Object Value { get; set; }
    }
}

