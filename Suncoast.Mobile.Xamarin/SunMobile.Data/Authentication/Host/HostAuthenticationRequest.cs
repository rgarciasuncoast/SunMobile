using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Host
{
    [DataContract]
    public class HostAuthenticationRequest : AuthenticationRequest
    {
        [DataMember]
        public string Password { get; set; }
    }
}
