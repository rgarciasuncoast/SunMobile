using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Session
{
    [DataContract]
    public class SessionRequest : AuthenticationRequest
    {
        [DataMember]
        public string UserId { get; set; }
    }
}
