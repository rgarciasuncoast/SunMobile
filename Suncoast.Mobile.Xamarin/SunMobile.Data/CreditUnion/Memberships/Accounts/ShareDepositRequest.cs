using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class ShareDepositRequest
    {
        public string CommandType { get; set; }
        [DataMember]
        public string TellerPassword { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public int ToAccount { get; set; }
        [DataMember]
        public string ToSuffix { get; set; }
        [DataMember]
        public string DepositHoldCode { get; set; }
        [DataMember]
        public int TraceNumber { get; set; }
    }
}