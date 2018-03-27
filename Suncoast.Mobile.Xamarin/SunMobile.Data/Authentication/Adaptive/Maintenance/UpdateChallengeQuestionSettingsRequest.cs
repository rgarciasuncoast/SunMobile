using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Maintenance
{
    [DataContract]
    public class UpdateChallengeQuestionSettingsRequest : AuthenticationRequest
    {
        [DataMember]
        public string SelectedQuestion1 { get; set; }
        [DataMember]
        public string SelectedQuestion2 { get; set; }
        [DataMember]
        public string SelectedQuestion3 { get; set; }
        [DataMember]
        public string SelectedAnswer1 { get; set; }
        [DataMember]
        public string SelectedAnswer2 { get; set; }
        [DataMember]
        public string SelectedAnswer3 { get; set; }
    }
}