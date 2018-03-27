using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ENoticeResponse
    {
        [DataMember]
        public string DocumentString { get; set; }
    }
}
