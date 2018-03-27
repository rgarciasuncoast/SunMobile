using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    /// <summary>
    /// Base class for all responses containing Authentication State
    /// </summary>
    [DataContract]
    public class ValidateAuthorizationTokenResponse : StatusResponse
    {
        [DataMember]
        public string MemberId { get; set; }
    }
}
