using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogStopPaymentItem
    {
        [DataMember]
        [SensitiveData]
        public string AccountDescription { get; set; }

        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        [SensitiveData]
        public string AccountSuffix { get; set; }

        [DataMember]
        public string AccountCollateralCode { get; set; }

        [DataMember]
        public bool isSingle { get; set; }

        [DataMember]
        public bool isRange { get; set; }

        [DataMember]
        [SensitiveData]
        public string CheckNumber { get; set; }

        [DataMember]
        [SensitiveData]
        public string CheckNumberRangeStart { get; set; }

        [DataMember]
        [SensitiveData]
        public string CheckNumberRangeEnd { get; set; }

    }
}
