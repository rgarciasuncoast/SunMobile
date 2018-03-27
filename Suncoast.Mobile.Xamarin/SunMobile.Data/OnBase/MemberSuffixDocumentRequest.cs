using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class MemberSuffixDocumentRequest : MemberDocumentRequest
    {
        [DataMember]
        public string Suffix { get; set; }
    }
}
