using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class IsMemberEnrolledInAchTransfersResponse : StatusResponse
    {
        [DataMember]
        public bool IsEnrolled { get; set; }
        [DataMember]
        public string Agreement { get; set; }
    }
}