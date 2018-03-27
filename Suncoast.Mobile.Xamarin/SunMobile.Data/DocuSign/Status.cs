using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class Status
    {
        [DataMember]
        public int PacketGroupId { get; set; }
        [DataMember]
        public int PacketId { get; set; }
        [DataMember]
        // ReSharper disable InconsistentNaming
        public string SSN { get; set; }
        // ReSharper restore InconsistentNaming
        [DataMember]
        public int ClientUserId { get; set; }
        [DataMember]
        public int StepOrder { get; set; }
        [DataMember]
        public string StatusText { get; set; }
        [DataMember]
        public bool EmailSent { get; set; }
        [DataMember]
        public DateTime EmailSentDate { get; set; }
        [DataMember]
        public bool EmailCompletedSent { get; set; }
        [DataMember]
        public DateTime EmailCompletedDate { get; set; }
        [DataMember]
        public SignerInformation SignerInfo { get; set; }
    }
}
