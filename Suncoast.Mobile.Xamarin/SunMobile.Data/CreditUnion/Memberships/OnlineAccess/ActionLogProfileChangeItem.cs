using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{    
    [DataContract]
    public class ActionLogProfileChangeItem
    {
        [DataMember]
        [SensitiveData]
        public string Address1 { get; set; }

        [DataMember]
        [SensitiveData]
        public string Address2 { get; set; }

        [DataMember]
        [SensitiveData]
        public string City { get; set; }

        [DataMember]
        [SensitiveData]
        public string State { get; set; }

        [DataMember]
        [SensitiveData]
        public string ZipCode { get; set; }

        [DataMember]
        [SensitiveData]
        public string HomePhoneNumberHome { get; set; }

        [DataMember]
        [SensitiveData]
        public string PhoneNumberWork { get; set; }

        [DataMember]
        [SensitiveData]
        public string PhoneNumberCell { get; set; }

        [DataMember]
        [SensitiveData]
        public string EmailAddress { get; set; }

        [DataMember]
        [SensitiveData]
        public string EmailAddressConfirmation { get; set; }
    }
}
