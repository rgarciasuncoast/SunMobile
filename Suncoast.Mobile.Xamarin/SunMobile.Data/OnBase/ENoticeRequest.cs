using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ENoticeRequest
    {
        [DataMember]
        public int DocumentId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Key { get; set; }
    }
}
