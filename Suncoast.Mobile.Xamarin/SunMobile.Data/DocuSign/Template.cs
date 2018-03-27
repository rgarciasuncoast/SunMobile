using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class Template
    {
        [DataMember]
        public int TemplateId { get; set; }
        [DataMember]
        public string DocuSignTemplateId { get; set; }
        [DataMember]
        public string TemplateDescription { get; set; }
        [DataMember]
        public string DocumentType { get; set; }
        [DataMember]
        public string EmailSubject { get; set; }
        [DataMember]
        public string EmailMessage { get; set; }
        [DataMember]
        public string CompletedEmailSubject { get; set; }
        [DataMember]
        public string CompletedEmailMessage { get; set; }
    }
}
