using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Fics
{
    [DataContract]
    public class History
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string TransactionType { get; set; }
        [DataMember]
        public DateTime DueDate { get; set; }
        [DataMember]
        public DateTime PaidDate { get; set; }
        [DataMember]
        public decimal PaymentAmount { get; set; }
        [DataMember]
        public string TransactionDescription { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public decimal Principal { get; set; }
        [DataMember]
        public decimal Interest { get; set; }
        [DataMember]
        public decimal TaxAndInsurance { get; set; }
        [DataMember]
        public DateTime ActualDateTime { get; set; }
        [DataMember]
        public string Bank { get; set; }
        [DataMember]
        public string Investor { get; set; }
        [DataMember]
        public string Group { get; set; }
        [DataMember]
        public string InvestorLoanNumber { get; set; }
        [DataMember]
        public decimal PrincipalBalance { get; set; }
        [DataMember]
        public decimal TaxAndInsuranceBalance { get; set; }
        [DataMember]
        public string ReversedFlag { get; set; }
        [DataMember]
        public decimal Curtailment { get; set; }
    }
}
