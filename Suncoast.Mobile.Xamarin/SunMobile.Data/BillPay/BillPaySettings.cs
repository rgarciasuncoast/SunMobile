using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.HolidaySchedule;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class BillPaySettings
    {
        #region Declarations...

        #endregion

        #region Properties...
       
        [DataMember]
        public decimal MinPaymentAmount { get; set; }

        [DataMember]
        public decimal MaxPaymentAmount { get; set; }

        [DataMember]
        public Int32 MaxPaymentFutureDaysForPayment { get; set; }

        [DataMember]
        public Int32 PaymentMethodElectronicDeliveryDays { get; set; }

        [DataMember]
        public Int32 PaymentMethodCheckDeliveryDays { get; set; }

        [DataMember]
        public List<Holiday> Holidays { get; set; } 

        #endregion

        #region Methods...

        #endregion
    }
}
