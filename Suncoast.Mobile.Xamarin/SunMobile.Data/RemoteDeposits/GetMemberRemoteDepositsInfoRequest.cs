using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class GetMemberRemoteDepositsInfoRequest
    {
        [DataMember]
        public string MemberId { get; set; }
    }
}