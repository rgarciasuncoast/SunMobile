using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class MicrNumber
    {
        [DataMember]
        public string MicrAccountNumber { get; set; }
    }
}
