using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class GetCheckImageFromDatabaseResponse : StatusResponse
    {
        [DataMember]
        public string Front { get; set; }

         [DataMember]
        public string Back { get; set; }
    }
}