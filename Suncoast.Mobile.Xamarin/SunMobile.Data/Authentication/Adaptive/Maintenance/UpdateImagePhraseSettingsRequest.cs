using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Maintenance
{
    [DataContract]
    public class UpdateImagePhraseSettingsRequest : AuthenticationRequest
    {
        [DataMember]
        public string SelectedImage { get; set; }

        [DataMember]
        public string SelectedPhrase { get; set; }

        [DataMember]
        public bool RetrieveNewImages { get; set; }
    }
}