using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class DepositCheckResponse : StatusResponse
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public string RejectReason { get; set; }
        [DataMember]
        public CheckStatusResponse CheckStatus { get; set; }
    }
}