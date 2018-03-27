using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.HolidaySchedule
{
    [DataContract]
    public class Holiday
    {
        [DataMember]
        [Queryable]
        public string Description { get; set; }
        [DataMember]
        public DateTime DateObserved { get; set; }
        [DataMember]
        public HolidayHours WorkingHours { get; set; }
    }
}
