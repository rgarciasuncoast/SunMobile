using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class PacketGroup
    {
        [DataMember]
        public int PacketGroupId { get; set; }
        [DataMember]
        public int AppId { get; set; }
        [DataMember]
        public int BusinessProcessId { get; set; }
        [DataMember]
        public DateTime DateStarted { get; set; }
        [DataMember]
        public DateTime DateCompleted { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public DateTime StatusUpdated { get; set; }
        [DataMember]
        public List<Packet> PacketCollection { get; set; }
    }
}