using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class ChallengeResponse : AuthenticationResponse
    {
        [DataMember]
        public string ChallengeQuestion { get; set; }
        [DataMember]
        public string SecurityPhrase { get; set; }
        [DataMember]
        public string SecurityImage { get; set; }
    }
}