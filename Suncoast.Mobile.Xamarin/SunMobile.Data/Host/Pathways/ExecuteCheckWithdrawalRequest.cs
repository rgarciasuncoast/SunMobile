using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
    [DataContract]
    public class ExecuteCheckWithdrawalRequest
    {
        [DataMember]
        public string FromSuffix { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string SorL { get; set; }
    }
}
