using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;
using SunBlock.DataTransferObjects.Culture;

namespace SunBlock.DataTransferObjects.Authentication
{
    [DataContract]
    public class AuthenticateFingerprintRequest
    {
        [DataMember]
        public string FingerprintAuthorizationCode { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public PayloadMessage Payload { get; set; }
        [DataMember]
        public string Language { get; set; }
    }
}