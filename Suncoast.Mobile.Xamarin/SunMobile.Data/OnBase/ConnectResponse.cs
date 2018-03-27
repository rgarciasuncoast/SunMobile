using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ConnectResponse : SunBlock.DataTransferObjects.StatusResponse
    {
        [DataMember]
        public string SessionId { get; set; }
    }
}