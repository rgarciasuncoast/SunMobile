using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class UpdateImagePhraseResponse : AuthenticationResponse
    {
        [DataMember]
        public string[] AvailableImages { get; set; }
        [DataMember]
        public string SecurityPhrase { get; set; }
        [DataMember]
        public string SecurityImage { get; set; }
    }
}