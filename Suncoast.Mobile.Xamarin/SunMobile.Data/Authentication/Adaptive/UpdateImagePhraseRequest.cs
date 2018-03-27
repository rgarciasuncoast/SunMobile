using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class UpdateImagePhraseRequest : AuthenticationRequest
    {
        [DataMember]
        public string SelectedImage { get; set; }

        [DataMember]
        public string SelectedPhrase { get; set; }

        [DataMember]
        public bool RetrieveNewImages { get; set; }
    }
}