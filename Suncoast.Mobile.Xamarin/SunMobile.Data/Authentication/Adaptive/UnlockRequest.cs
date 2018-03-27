
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class UnlockRequest : AuthenticationRequest
    {
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
