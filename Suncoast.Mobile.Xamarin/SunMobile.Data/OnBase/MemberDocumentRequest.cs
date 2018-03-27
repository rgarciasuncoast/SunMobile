using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class MemberDocumentRequest : DocumentRequest
    {
        [DataMember]
        public int MemberId { get; set; }
    }
}
