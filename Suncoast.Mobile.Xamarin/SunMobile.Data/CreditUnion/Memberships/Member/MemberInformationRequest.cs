using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Member
{
    [DataContract]
    public class MemberInformationRequest
    {
        [DataMember]
        public int MemberId { get; set; }
    }
}