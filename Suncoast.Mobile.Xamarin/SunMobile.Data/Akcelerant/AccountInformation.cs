using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Akcelerant
{
    [DataContract]
    public class AccountInformation
    {

        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public string AccountId { get; set; }

    }
}
