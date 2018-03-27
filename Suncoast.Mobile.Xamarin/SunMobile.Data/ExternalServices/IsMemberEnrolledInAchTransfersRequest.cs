using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class IsMemberEnrolledInAchTransfersRequest
    {
        [DataMember]
        public int MemberId { get; set; }
    }
}