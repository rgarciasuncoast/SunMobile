using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class UpdateChallengeQuestionsResponse : AuthenticationResponse
    {
        [DataMember]
        public string[] AvailableImages { get; set; }

    }
}