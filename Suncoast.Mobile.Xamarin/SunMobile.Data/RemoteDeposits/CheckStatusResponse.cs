using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class CheckStatusResponse : StatusResponse
    {
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public CheckStatus CheckStatusInfo { get; set; }
    }
}