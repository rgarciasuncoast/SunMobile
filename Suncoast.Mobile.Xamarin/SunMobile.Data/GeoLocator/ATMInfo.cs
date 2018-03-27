using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.GeoLocator
{
    [DataContract]
    public class AtmInfo : Location
    {
        [DataMember]
        public string AtmID { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public bool Deposit { get; set; }

        [DataMember]
        public bool PublicAccess { get; set; }

        [DataMember]
        public bool Operating { get; set; }

        [DataMember]
        [Queryable]
        public bool Suncoast { get; set; }

        [DataMember]
        [Queryable]
        public int RevDate { get; set; }
    }
}
