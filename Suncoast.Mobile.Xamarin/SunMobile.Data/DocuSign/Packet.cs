using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class Packet
    {
        [DataMember]
        public int PacketId { get; set; }
        [DataMember]
        public int PacketGroupId { get; set; }
        [DataMember]
        public string EnvelopeId { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public byte[] FileBytes { get; set; }
        [DataMember]
        public int TemplateId { get; set; }
        [DataMember]
        public List<Status> StatusCollection { get; set; }
    }
}