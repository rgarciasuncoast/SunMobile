using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    /// <summary>
    /// Base class for all responses containing Authentication State
    /// </summary>
    [DataContract]
    public class AuthorizedResponse
    {
        [DataMember]
        public AuthorizationStates CurrentState {get; set;}
    }
}
