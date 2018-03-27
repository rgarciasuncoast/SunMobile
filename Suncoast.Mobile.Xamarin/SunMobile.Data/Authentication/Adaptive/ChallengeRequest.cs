using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class ChallengeRequest : AuthenticationRequest
    {
        [DataMember]
        public string ChallengeAnswer { get; set; }

        [DataMember]
        public bool BindDevice { get; set; }
    }
}