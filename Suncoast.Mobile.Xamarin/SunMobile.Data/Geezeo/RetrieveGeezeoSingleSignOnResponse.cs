using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Geezeo
{
    [DataContract]
    public class RetrieveGeezeoSingleSignOnResponse : GeezeoResponseBase
    {
        [DataMember]
        public string SingleSignOnResponse { get; set; }
    }
}