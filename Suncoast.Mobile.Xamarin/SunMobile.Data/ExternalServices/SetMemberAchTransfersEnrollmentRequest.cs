using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class SetMemberAchTransfersEnrollmentRequest
    {
        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public string MemberCode { get; set; }

        [DataMember]
        public MemberInformation MemberInfo { get; set; }

        [DataMember]
        public bool IsEnrolled { get; set; }
    }
}