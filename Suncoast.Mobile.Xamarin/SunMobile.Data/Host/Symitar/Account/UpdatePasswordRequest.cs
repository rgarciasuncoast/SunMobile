using System.Runtime.Serialization;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
    [DataContract]
    public class UpdatePasswordRequest
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}