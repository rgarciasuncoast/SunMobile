using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.GeoLocator
{
    [DataContract]
    public class LocationInfo : Location
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string LocationId { get; set; }
        [DataMember]
        public string LocationName { get; set; }
        [DataMember]
        public string StateAbbr { get; set; }
        [DataMember]
        public Dictionary<string, string> Details { get; set; }
    }
}
