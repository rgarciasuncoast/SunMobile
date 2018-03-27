using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogEnrollmentItem
    {
        [DataMember]
        public string ButtonClicked { get; set; }

        [DataMember]
        [SensitiveData]
        public string EmailAddress { get; set; }

        [DataMember]
        [SensitiveData]
        public string EmailAddressConfirmation { get; set; }

        [DataMember]        
        public bool IsSpanish { get; set; }
    }
}
