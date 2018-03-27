using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Geezeo
{
    [DataContract]
    public class IsMemberEnrolledInSunMoneyResponse : GeezeoResponseBase
    {
        [DataMember]
        public bool IsMemberEnrolledInSunMoney { get; set; }
        [DataMember]
        public string SunMoneyAgreement { get; set; }
    }
}