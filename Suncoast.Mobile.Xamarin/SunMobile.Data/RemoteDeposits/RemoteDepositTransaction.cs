using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class RemoteDepositTransaction
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public DateTime TransactionDate { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public byte[] CheckFront { get; set; }
        [DataMember]
        public byte[] CheckBack { get; set; }
    }
}