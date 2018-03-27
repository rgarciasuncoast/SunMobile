using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class CheckImageDocument
    {
        [DataMember]
        public int DocumentId { get; set; }
        [DataMember]
        public string DocumentName { get; set; }
        [DataMember]
        public byte[] FrontImage { get; set; }
        [DataMember]
        public byte[] BackImage { get; set; }
    }
}