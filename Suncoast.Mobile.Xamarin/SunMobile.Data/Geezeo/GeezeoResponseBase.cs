using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Geezeo
{
    [DataContract]
    public class GeezeoResponseBase
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string FailureMessage { get; set; }
    }
}
