using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class NotEnrolledResponse : AuthenticationResponse
    {
        [DataMember]
        public string[] EnrollQuestionSet1 { get; set; }
        [DataMember]
        public string[] EnrollQuestionSet2 { get; set; }
        [DataMember]
        public string[] EnrollQuestionSet3 { get; set; }
    }
}