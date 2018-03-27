using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class RetrieveAchSingleSignOnResponse : StatusResponse
    {
        [DataMember]
        public string SingleSignOnResponse { get; set; }
    }
}