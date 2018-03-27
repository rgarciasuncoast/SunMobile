using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogBillPayPayeeItem
    {
        [DataMember]
        [SensitiveData]
        public String Name { get; set; }

        [DataMember]
        [SensitiveData]
        public string AccountNumber { get; set; }

        [DataMember]
        [SensitiveData]
        public string UserPayeeId { get; set; }

        [DataMember]
        [SensitiveData]
        public string PayeeId { get; set; }

        [DataMember]
        [SensitiveData]
        public string NickName { get; set; }

        [DataMember]
        [SensitiveData]
        public string UniversalNameOnBill { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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
        public string PhoneNumber { get; set; }

    }
}
