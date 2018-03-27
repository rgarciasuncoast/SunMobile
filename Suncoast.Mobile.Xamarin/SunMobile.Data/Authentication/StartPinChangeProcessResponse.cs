using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    /// <summary>
    /// Base class for all responses containing Authentication State
    /// </summary>
    [DataContract]
    public class StartPinChangeProcessResponse : StatusResponse
    {
        /// <summary>
        /// The long & complicated token used in the URL
        /// </summary>
        [DataMember]
        public string SecurityToken { get; set; }

        /// <summary>
        /// The short & user firendly token that a member will have to manually enter
        /// </summary>
        [DataMember]
        public string AuthorizationToken { get; set; }
    }
}
