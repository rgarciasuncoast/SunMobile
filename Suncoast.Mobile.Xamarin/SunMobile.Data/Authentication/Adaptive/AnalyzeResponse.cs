using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class AnalyzeResponse : Session.SessionResponse
    {
        [DataMember]
        public string SecurityImage { get; set; }
        [DataMember]
        public string SecurityPhrase { get; set; }
        [DataMember]
        public string ChallengeQuestion { get; set; }
        [DataMember]
        public string[] EnrollQuestionSet1 { get; set; }
        [DataMember]
        public string[] EnrollQuestionSet2 { get; set; }
        [DataMember]
        public string[] EnrollQuestionSet3 { get; set; }
    }
}
