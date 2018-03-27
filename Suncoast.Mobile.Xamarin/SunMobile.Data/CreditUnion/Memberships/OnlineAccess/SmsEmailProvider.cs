using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class SmsEmailProvider
    {
        [DataMember]
        public string ProviderName { get; set; }
        [DataMember]
        public string ProviderAddress { get; set; }
    }
}
