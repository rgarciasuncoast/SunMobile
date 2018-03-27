using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects
{
    [DataContract]
    public class StatusResponse
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string FailureMessage { get; set; }
    }

    [DataContract]
    public class StatusResponse<T>: StatusResponse
    {
        [DataMember]
        public T Result { get; set; }
    }
}