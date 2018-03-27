using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ConnectRequest
    {
        [DataMember]
        public bool AllowWriteAccess { get; set; }
    }
}