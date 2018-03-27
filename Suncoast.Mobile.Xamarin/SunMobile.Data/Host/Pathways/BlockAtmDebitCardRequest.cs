using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
    [DataContract]
    public class BlockAtmDebitCardRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public int CardNumber { get; set; }
        [DataMember]
        public bool IsAtmCard { get; set; }
        [DataMember]
        public bool IsLost { get; set; }
        [DataMember]
        public string Memo { get; set; }
        [DataMember]
        public string CardAccountNumber { get; set; }
    }
}
