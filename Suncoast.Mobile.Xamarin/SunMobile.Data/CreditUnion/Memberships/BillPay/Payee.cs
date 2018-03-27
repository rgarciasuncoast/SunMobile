using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.BillPay
{
    [DataContract]
    public class Payee
    {
        [DataMember]
        public int PayeeId { get; set; }
        [DataMember]
        public string PayeeName { get; set; }
        [DataMember]
        public string PayeeNickName { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string NameOnAccount { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string PaymentMethod { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public DateTime LastUpdatedDate { get; set; }
        [DataMember]
        public DateTime LastPaymentSendDate { get; set; }
        [DataMember]
        public decimal LastPaymentAmount { get; set; }
        [DataMember]
        public bool Deleted { get; set; }
        [DataMember]
        public int BillPresentmentAccountId { get; set; }
        [DataMember]
        public int BillPresentmentId { get; set; }
        [DataMember]
        public string InstitutionName { get; set; }
        [DataMember]
        public int InstitutionId { get; set; }
        [DataMember]
        public bool Global { get; set; }
    }
}