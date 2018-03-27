using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogTransferItem
    {
        [DataMember]
        [SensitiveData]
        public string ToAccount { get; set; }

        [DataMember]        
        public string ToAccountType { get; set; }

        [DataMember]
        [SensitiveData]
        public string ToAccountSuffix { get; set; }

        [DataMember]
        [SensitiveData]
        public string ToAccountDescription { get; set; }

        [DataMember]
        [SensitiveData]
        public string ToAccountCollateralCode { get; set; }

        [DataMember]
        public bool ToIsShare { get; set; }

        [DataMember]
        [SensitiveData]
        public string ToLastName { get; set; }

        [DataMember]
        [SensitiveData]
        public string FromAccount { get; set; }

        [DataMember]        
        public string FromAccountType { get; set; }

        [DataMember]
        [SensitiveData]
        public string FromAccountSuffix { get; set; }

        [DataMember]
        [SensitiveData]
        public string FromAccountDescription { get; set; }

        [DataMember]        
        public string FromAccountCollateralCode { get; set; }

        [DataMember]
        public bool FromIsShare { get; set; }

        [DataMember]
        public string Amount { get; set; }

        [DataMember]
        [SensitiveData]
        public string ConfirmationDetails { get; set; }
    }
}
