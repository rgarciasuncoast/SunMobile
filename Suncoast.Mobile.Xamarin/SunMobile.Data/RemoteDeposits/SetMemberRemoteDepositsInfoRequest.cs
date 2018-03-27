using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class SetMemberRemoteDepositsInfoRequest
    {
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public bool AgreedToTerms { get; set; }
        [DataMember]
        public bool SmsAlertsEnabled { get; set; }
    }
}