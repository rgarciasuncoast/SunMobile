using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Akcelerant
{
    [DataContract]
    public class BorrowerInformation
    {
        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }
}
