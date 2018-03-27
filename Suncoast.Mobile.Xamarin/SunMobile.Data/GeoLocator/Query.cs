using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.GeoLocator
{
    [DataContract]
    public class Query : Location
    {
        [DataMember]
        public string Type { get; set; }
    }
}
