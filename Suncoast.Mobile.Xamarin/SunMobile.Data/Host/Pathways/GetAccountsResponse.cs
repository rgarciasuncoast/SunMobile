using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
    [DataContract]
    public class GetAccountsResponse : StatusResponse
    {
        [DataMember]
        public AccountList Accounts { get; set; }
    }
}
