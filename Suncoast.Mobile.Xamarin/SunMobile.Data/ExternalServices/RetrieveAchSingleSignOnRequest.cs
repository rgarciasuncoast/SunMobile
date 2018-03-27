using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class RetrieveAchSingleSignOnRequest
    {
        [DataMember]
        public string MemberCode { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public MemberInformation MemberInfo { get; set; }
        
    }
}