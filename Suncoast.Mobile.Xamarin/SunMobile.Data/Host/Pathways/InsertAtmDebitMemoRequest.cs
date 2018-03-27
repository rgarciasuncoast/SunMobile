using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
    [DataContract]
    public class InsertAtmDebitMemoRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public int CardNumber { get; set; }
        [DataMember]
        public string CardAccountNumber { get; set; }
        [DataMember]
        public bool IsAtmCard { get; set; }
        [DataMember]
        public string Memo { get; set; }
    }
}
