using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class DisconnectRequest
    {
        [DataMember]
        public string SessionId { get; set; }
    }
}