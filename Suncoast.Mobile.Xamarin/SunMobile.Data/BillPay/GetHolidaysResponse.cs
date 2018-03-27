using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.HolidaySchedule;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetHolidaysResponse : BillPayResponseBase
    {
        [DataMember]
        public List<Holiday> Holidays { get; set; }
    }
}