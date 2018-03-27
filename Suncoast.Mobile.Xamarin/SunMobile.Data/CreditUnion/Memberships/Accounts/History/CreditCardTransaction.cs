using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.History
{
    [DataContract]
    public class CreditCardTransaction : AccountTransaction
    {
        [DataMember]
        public string Reference { get; set; }
    }
}
