using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class CheckStatusRequest
    {
        [DataMember]
        public string UserNameToken { get; set; }
        [DataMember]
        public long TransactionId { get; set; }
        [DataMember]
        public bool ReturnCheckData { get; set; }
        [DataMember]
        public bool ReturnFrontImage { get; set; }
        [DataMember]
        public bool ReturnBackImage { get; set; }
    }
}