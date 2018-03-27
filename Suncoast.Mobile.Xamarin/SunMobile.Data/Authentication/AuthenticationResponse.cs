using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    [DataContract]
    public class AuthenticationResponse : AuthorizedResponse
    {
        [DataMember]
        public string DeviceTokenPrimaryStorage { get; set; }
        [DataMember]
        public string DeviceTokenSecondaryStorage { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public string SessionId { get; set; }

    }
}
