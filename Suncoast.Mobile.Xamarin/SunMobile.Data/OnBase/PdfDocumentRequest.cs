using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class PdfDocumentRequest
    {
        [DataMember]
        public string DocumentId { get; set; }
        [DataMember]
        public bool Resize { get; set; }
    }
}
