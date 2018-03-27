using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Geezeo
{
    [DataContract]
    public class RetrieveGeezeoSingleSignOnRequest
    {
        [DataMember]
        public string IssuerUrl { get; set; }
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public bool IsMobile { get; set; }
    }
}