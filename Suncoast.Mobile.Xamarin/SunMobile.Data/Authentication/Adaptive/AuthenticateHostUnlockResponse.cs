using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class AuthenticateHostUnlockResponse : Host.HostAuthenticationResponse
    {
        [DataMember]
        public string[] UnlockQuestionSet1 { get; set; }
        [DataMember]
        public string[] UnlockQuestionSet2 { get; set; }
        [DataMember]
        public string[] UnlockQuestionSet3 { get; set; }
        [DataMember]
        public string SelectedQuestion1 { get; set; }
        [DataMember]
        public string SelectedQuestion2 { get; set; }
        [DataMember]
        public string SelectedQuestion3 { get; set; }
        [DataMember]
        public string UnlockAnswer1 { get; set; }
        [DataMember]
        public string UnlockAnswer2 { get; set; }
        [DataMember]
        public string UnlockAnswer3 { get; set; }

    }
}
