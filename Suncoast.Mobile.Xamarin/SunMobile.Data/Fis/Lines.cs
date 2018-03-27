using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Fis
{
    [DataContract]
    public class Lines
    {
        [DataMember]
        public string ReferenceNumber { get; set; }
        [DataMember]
        public DateTime PostMonthDay { get; set; }
        [DataMember]
        public DateTime TransactionMonthDay { get; set; }
        [DataMember]
        public string ItemMessage { get; set; }
        [DataMember]
        public string ItemAmountString { get; set; }
        [DataMember]
        public decimal ItemAmount { get; set; }
        [DataMember]
        public string StatementKey { get; set; }
        [DataMember]
        public string TransactionCode { get; set; }
        [DataMember]
        public string ExpandIndicator { get; set; }
        [DataMember]
        public string CardType { get; set; }
    }
}
