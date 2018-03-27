using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Maintenance
{
    [DataContract]
    public class RetrieveSettingsResponse : Host.HostAuthenticationResponse
    {
        [DataMember]
        public string SecurityImage { get; set; }

        [DataMember]
        public string SecurityImagePath { get; set; }

        [DataMember]
        public string SecurityPhrase { get; set; }

        [DataMember]
        public string[] QuestionSet1 { get; set; }
        [DataMember]
        public string[] QuestionSet2 { get; set; }
        [DataMember]
        public string[] QuestionSet3 { get; set; }
        [DataMember]
        public string SelectedQuestion1 { get; set; }
        [DataMember]
        public string SelectedQuestion2 { get; set; }
        [DataMember]
        public string SelectedQuestion3 { get; set; }
        [DataMember]
        public string Answer1 { get; set; }
        [DataMember]
        public string Answer2 { get; set; }
        [DataMember]
        public string Answer3 { get; set; }

    }
}