using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.Fis
{
    [DataContract]
    public class CardInformation
    {
        [DataMember]
        [SensitiveData]
        public string CreditCardNumber { get; set; }
        [DataMember]
        public GeneralCardHolderInformation GeneralCardholderInfo { get; set; }
    }
}
