using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class Payment
    {
        [DataMember]
        public long PaymentId { get; set; }
        [DataMember]
        public string PaymentMethod { get; set; }
        [DataMember]
        public int RunId { get; set; }
        [DataMember]
        public string PaymentStatusErrorCode { get; set; }
        [DataMember]
        public string PaymentStatusErrorDescription { get; set; }
        [DataMember]
        public long RecurringId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public DateTime WillProcessDate { get; set; }
        [DataMember]
        public DateTime PaymentProcessDate { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string Memo { get; set; }
        [DataMember]
        public string PaymentBillReference { get; set; }
        [DataMember]
        public DateTime EntryDate { get; set; }
        [DataMember]
        public DateTime DueDate { get; set; }
        [DataMember]
        public DateTime FailedDate { get; set; }
        [DataMember]
        public DateTime CanceledDate { get; set; }
        [DataMember]
        public DateTime SuccessDate { get; set; }
        [DataMember]
        public DateTime SearchBeginDate { get; set; }
        [DataMember]
        public bool IsPending { get; set; }

        // Payee Properties
        [DataMember]
        public long PayeeId { get; set; }
        [DataMember]
        public string PayeeName { get; set; }
        [DataMember]
        public string PayeeNickName { get; set; }
        [DataMember]
        public string PayeeAccountNumber { get; set; }

        // Recurring Properties
        [DataMember]
        public long RecPaymentId { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public DateTime LastProcessedDate { get; set; }
        [DataMember]
        public DateTime InitialDueDate { get; set; }
        [DataMember]
        public int NumPayments { get; set; }
        [DataMember]
        public int NumPaymentsLeft { get; set; }
        [DataMember]
        public string Frequency { get; set; }
        [DataMember]
        public int FrequencyId { get; set; }
        [DataMember]
        public DateTime LastUpdateDateTime { get; set; }
        [DataMember]
        public string SourceApplication { get; set; }
        [DataMember]
        public DateTime NextDueDate { get; set; }
        [DataMember]
        public bool IndefinitePayment { get; set; }
    }
}