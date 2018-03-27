using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class VerifyRemoteDepositInfoRequest
    {
        [DataMember]
        public string UserNameToken { get; set; }
        [DataMember]
        public long AmountInCents { get; set; }
    }
}