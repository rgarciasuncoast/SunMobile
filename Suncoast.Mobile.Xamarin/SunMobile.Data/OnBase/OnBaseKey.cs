using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class OnBaseKey
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}