using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class LoginRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
