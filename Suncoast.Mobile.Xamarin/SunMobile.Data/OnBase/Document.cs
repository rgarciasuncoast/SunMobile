using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    /// <summary>
    /// Represents the base class from which all Onbase document classes derive.
    /// </summary>
    [DataContract]
    public class Document
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public int DocumentId { get; set; }
        [DataMember]
        public string DocumentName { get; set; }
        [DataMember]
        public string MessageQueueId { get; set; }
        [DataMember]
        public bool IsSuccessful { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public byte[] PageData { get; set; }
        [DataMember]
        public string FileTypeName { get; set; }
    }
}