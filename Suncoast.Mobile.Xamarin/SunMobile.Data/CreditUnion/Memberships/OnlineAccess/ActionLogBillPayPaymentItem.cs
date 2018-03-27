using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogBillPayPaymentItem
    {
        [DataMember]       
        public ActionLogBillPayPayeeItem Payee { get; set; }      

        [DataMember]
        public string SendOnDate { get; set; }

        [DataMember]
        public string DeliverByDate { get; set; }

        [DataMember]
        public string Amount { get; set; }

        [DataMember]
        [SensitiveData]
        public string Suffix { get; set; }

        [DataMember]
        public string Frequency { get; set; }

        [DataMember]
        public string NumberOfPayments { get; set; }

        [DataMember]
        public string DeliveryMethod { get; set; }
     
        [DataMember]
        public bool IsActive { get; set; }

    }
}
