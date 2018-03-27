using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
    [DataContract]
    public class PayoffQuoteResponse : StatusResponse
    {
        [DataMember]
        public decimal TotalPayoffAmount { get; set; }

        [DataMember]
        public string PayOffDateGoodThrough { get; set; }

        [DataMember]
        public string LoanSuffix { get; set; }

        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string AccountDescription { get; set; }

        [DataMember]
        public string ViewTemplate { get; set; }

        [DataMember]
        public string LoanDescription { get; set; }
        [DataMember]
        public decimal PrincipalBalance { get; set; }
        [DataMember]
        public decimal InterestDue { get; set; }
        [DataMember]
        public string DueDate { get; set; }
        [DataMember]
        public string PastDuePaymentCount { get; set; }
        [DataMember]
        public decimal InterestRate { get; set; }
        [DataMember]
        public decimal OneDaysInterest { get; set; }
        [DataMember]
        public decimal AmountPastDuebyPayoffDate { get; set; }
        [DataMember]
        public string LateChargeDue { get; set; }
        [DataMember]
        public decimal TotalLoanPayoff { get; set; }
        [DataMember]
        public decimal PayoffAmount { get; set; }
    }
}
