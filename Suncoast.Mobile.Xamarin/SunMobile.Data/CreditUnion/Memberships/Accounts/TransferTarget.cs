using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class TransferTarget
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public string AccountType { get; set; }
    }
}
