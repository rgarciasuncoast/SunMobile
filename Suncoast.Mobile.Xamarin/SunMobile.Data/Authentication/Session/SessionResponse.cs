using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Session
{
    [DataContract]
    public class SessionResponse : AuthenticationResponse
    {
        [DataMember]
        public string Token { get; set; }
    }
}
