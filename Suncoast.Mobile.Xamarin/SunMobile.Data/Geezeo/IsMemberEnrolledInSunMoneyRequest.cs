using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Geezeo
{
    [DataContract]
    public class IsMemberEnrolledInSunMoneyRequest
    {
        [DataMember]
        public int MemberId { get; set; }
    }
}