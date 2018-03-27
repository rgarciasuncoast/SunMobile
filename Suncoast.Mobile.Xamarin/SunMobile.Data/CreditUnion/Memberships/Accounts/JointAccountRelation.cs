using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class JointAccountRelation
    {
        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public List<string> Suffixes { get; set; } 
    }
}
