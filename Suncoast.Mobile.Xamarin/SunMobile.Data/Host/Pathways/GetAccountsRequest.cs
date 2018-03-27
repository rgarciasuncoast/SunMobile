using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
    [DataContract]
    public class GetAccountsRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string TaxId { get; set; }
        [DataMember]
        public bool FilterRestrictedAccounts { get; set; }
        [DataMember]
        public bool GetRealTimeCertegyInformation { get; set; }
        [DataMember]
        public bool GetJointAccounts { get; set; }
    }
}
