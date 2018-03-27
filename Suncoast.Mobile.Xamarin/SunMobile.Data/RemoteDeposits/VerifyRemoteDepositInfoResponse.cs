using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class VerifyRemoteDepositInfoResponse : StatusResponse
    {
        [DataMember]
        public bool IsUserAllowed { get; set; }        
    }
}