using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
    [DataContract]
    public class QueuedCommand
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime EntryDate { get; set; }

        [DataMember]
        public string CommandString { get; set; }

        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public int StatusId { get; set; }

        [DataMember]
        public int TypeId { get; set; }
    }
}
