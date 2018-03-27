using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Maintenance
{
    [DataContract]
    public class UpdateImagePhraseSettingsResponse : AuthenticationResponse
    {
        [DataMember]
        public string[] AvailableImages { get; set; }
    }
}