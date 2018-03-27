using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Geezeo
{
    [DataContract]
    public class SetMemberSunMoneyEnrollmentRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public bool IsEnrolled { get; set; }
    }
}