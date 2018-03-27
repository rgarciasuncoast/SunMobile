using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class RemoteDepositTransactionRequest
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
