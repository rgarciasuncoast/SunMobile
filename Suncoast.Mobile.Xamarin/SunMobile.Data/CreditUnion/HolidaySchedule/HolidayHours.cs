using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.HolidaySchedule
{
    [DataContract]
    public class HolidayHours
    {
        [DataMember]
        public DateTime Open { get; set; }
        [DataMember]
        public DateTime Close { get; set; }
    }
}
