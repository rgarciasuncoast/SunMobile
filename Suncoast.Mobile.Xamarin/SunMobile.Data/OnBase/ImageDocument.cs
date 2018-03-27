using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ImageDocument : Document
    {
        [DataMember]
        public Collection<ImagePage> Images { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public ImageDocumentTypes DocumentType { get; set; }
    }
}