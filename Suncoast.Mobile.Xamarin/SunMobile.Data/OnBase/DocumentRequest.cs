using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public abstract class DocumentRequest
    {
        [DataMember]
        public DateTime BeginTimeQuery { get; set; }
        [DataMember]
        public DateTime EndTimeQuery { get; set; }
        [DataMember]
        public bool ExcludeDocumentData { get; set; }
    }
}
