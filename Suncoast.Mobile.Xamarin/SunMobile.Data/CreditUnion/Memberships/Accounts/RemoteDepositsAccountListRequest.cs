using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class RemoteDepositsAccountListRequest
    {
        [DataMember]
        public string MemberId { get; set; }
    }
}