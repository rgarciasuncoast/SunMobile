using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ImagePage
    {
        [DataMember]
        public byte[] ImageStream { get; set; }
    }
}