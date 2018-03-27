using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class CheckImagesResponse
    {
        [DataMember]
        public Collection<CheckImageDocument> CheckImageDocuments { get; set; }
    }
}