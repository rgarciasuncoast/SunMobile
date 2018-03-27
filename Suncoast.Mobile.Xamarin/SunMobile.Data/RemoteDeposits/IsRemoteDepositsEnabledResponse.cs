using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class IsRemoteDepositsEnabledResponse : StatusResponse
    {
        [DataMember]
        public bool IsRemoteDepositsEnabled { get; set; }
    }
}