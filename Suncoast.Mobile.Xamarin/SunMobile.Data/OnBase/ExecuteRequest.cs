using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ExecuteRequest
    {
        [DataMember]
        public string SessionId { get; set; }
        [DataMember]
        public RequestNames RequestName { get; set; }
        [DataMember]
        public string ContentType { get; set; }
        [DataMember]
        public bool RenderData { get; set; }
        [DataMember]
        public string DocumentType { get; set; }
        [DataMember]
        public string CustomQueryName { get; set; }
        [DataMember]
        public Collection<OnBaseKey> OnBaseKeys { get; set; }
        [DataMember]
        public DateTime QueryBeginTimeUtc { get; set; }
        [DataMember]
        public DateTime QueryEndTimeUtc { get; set; }
        [DataMember]
        public int DocumentIdBegin { get; set; }
        [DataMember]
        public int DocumentIdEnd { get; set; }
        [DataMember]
        public bool ExcludeDocumentData { get; set; }
        [DataMember]
        public string DocumentId { get; set; }
        [DataMember]
        public bool Overlay { get; set; }
    }
}