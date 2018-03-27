using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class AccountCredentials
    {
        [DataMember]
        public string ApiUrl { get; set; }

        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
