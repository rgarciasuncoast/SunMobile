using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class DocuSignStatusList
    {
        [DataMember]
        public int PacketGroupId { get; set; }
        [DataMember]
        public int PacketId { get; set; }
        [DataMember]
        public int AppId { get; set; }
        [DataMember]
        public string DocumentType { get; set; }
        [DataMember]
        public string EnvelopeId { get; set; }
        [DataMember]
        public string PacketGroupStatus { get; set; }
        [DataMember]
        public DateTime StatusUpdated { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public int StepOrder { get; set; }
        [DataMember]
        public int ClientUserId { get; set; }
        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public string RecipientUserName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string BorrowerType { get; set; }
        [DataMember]
        public string AccessCode { get; set; }
        [DataMember]
        public string RecipientStatus { get; set; }
        [DataMember]
        public bool EmailSent { get; set; }
        [DataMember]
        public DateTime EmailSentDate { get; set; }
        [DataMember]
        public bool EmailCompletedSent { get; set; }
        [DataMember]
        public DateTime EmailCompletedSentDate { get; set; }
    }
}