using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    [DataContract]
    public class AuthenticationRequest
    {
        [DataMember]
        public string DevicePrint { get; set; }
        [DataMember]
        public string DeviceTokenPrimaryStorage { get; set; }
        [DataMember]
        public string DeviceTokenSecondaryStorage { get; set; }
        [DataMember]
        public string HttpAccept { get; set; }
        [DataMember]
        public string HttpAcceptChars { get; set; }
        [DataMember]
        public string HttpAcceptEncoding { get; set; }
        [DataMember]
        public string HttpAcceptLanguage { get; set; }
        [DataMember]
        public string HttpReferrer { get; set; }
        [DataMember]
        public string UserAgent { get; set; }
        [DataMember]
        public string ClientIpAddress { get; set; }

    }
}
