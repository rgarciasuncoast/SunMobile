using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
    [DataContract]
    public class LocalIndexRate
    {
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public decimal Rate { get; set; }
        [DataMember]
        public decimal CutoffAmount { get; set; }
    }
}
