using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogLoginItem
    {
        [DataMember]       
        public string LastSignonDate { get; set; }

        [DataMember]
        public string CipherStrength { get; set; }

        [DataMember]
        public bool MustEnroll { get; set; }

        [DataMember]
        public bool BillPayActive { get; set; }

        [DataMember]
        [SensitiveData]
        public string UserDeviceInformation { get; set; }

        [DataMember]
        [SensitiveData]
        public string LoginAction { get; set; }
    }
}
