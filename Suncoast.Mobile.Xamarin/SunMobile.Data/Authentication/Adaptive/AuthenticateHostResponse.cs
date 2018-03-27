using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
    [DataContract]
    public class AuthenticateHostResponse : Host.HostAuthenticationResponse
    {

        [DataMember]
        public bool ChangePinRequired { get; set; }
    }
}