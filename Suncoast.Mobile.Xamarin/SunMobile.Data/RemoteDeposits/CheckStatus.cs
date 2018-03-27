using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class CheckStatus
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public string CheckStatusSummary { get; set; }
        [DataMember]
        public string CheckStatusDetail { get; set; }
        [DataMember]
        public bool DepositRejected { get; set; }
        [DataMember]
        public string MemoPostStatus { get; set; }
        [DataMember]
        public string PostingDate { get; set; }
        [DataMember]
// ReSharper disable InconsistentNaming
        public string WFCTracer { get; set; }
// ReSharper restore InconsistentNaming
    }
}