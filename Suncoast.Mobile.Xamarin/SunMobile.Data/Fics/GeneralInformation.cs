using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Fics
{
    [DataContract]
    public class GeneralInformation
    {
        [DataMember]
        public string RealTimeAccessVersion { get; set; }
        [DataMember]
        public DateTime ResponseDateTime { get; set; }
        [DataMember]
        public string LoanNumber { get; set; }
        [DataMember]
        public string Bank { get; set; }
        [DataMember]
        public string Investor { get; set; }
        [DataMember]
        public string Group { get; set; }
        [DataMember]
        public decimal InterestRate { get; set; }
        [DataMember]
        public string PaymentFrequency { get; set; }
        [DataMember]
        public decimal OriginalLoanAmount { get; set; }
        [DataMember]
        public DateTime FundingDate { get; set; }
        [DataMember]
        public DateTime MaturityDate { get; set; }
        [DataMember]
        public DateTime NextRateChangeDate { get; set; }
        [DataMember]
        public DateTime NextPaymentChangeDate { get; set; }
        [DataMember]
        public string PayHistory { get; set; }
        [DataMember]
        public string MemberNumber { get; set; }
        [DataMember]
        public decimal PrincipalPaidYearToDate { get; set; }
        [DataMember]
        public decimal InterestPaidYearToDate { get; set; }
        [DataMember]
        public decimal LateChargesPaidYearToDate { get; set; }
        [DataMember]
        public string LateChargeGracePeriod { get; set; }
        [DataMember]
        public decimal ForecastedLateCharge { get; set; }
        [DataMember]
        public DateTime LastPaymentDate { get; set; }
        [DataMember]
        public DateTime DateOfNote { get; set; }
        [DataMember]
        public string LoanType { get; set; }
        [DataMember]
        public string TermInMonths { get; set; }
        [DataMember]
        public string DailyInterestLoan { get; set; }
        [DataMember]
        public string BillingMethod { get; set; }
        [DataMember]
        public string LoanPlanName { get; set; }
        [DataMember]
        public DateTime DueDateNextPayment { get; set; }
        [DataMember]
        public decimal PrincipalAndInterestPayment { get; set; }
        [DataMember]
        public decimal TotalPayment { get; set; }
        [DataMember]
        public decimal TotalDue { get; set; }
        [DataMember]
        public decimal PrincipalBalance { get; set; }
        [DataMember]
        public decimal NetDeferredPrincipalBalance { get; set; }
        [DataMember]
        public decimal UnappliedBalance { get; set; }
    }
}
