using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.GeoLocator
{
    [DataContract]
    public class Location
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Zip { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        [Queryable]
        public double Latitude { get; set; }

        [DataMember]
        [Queryable]
        public double Longitude { get; set; }

        [DataMember]
        public double Distance { get; set; }
    }
}
