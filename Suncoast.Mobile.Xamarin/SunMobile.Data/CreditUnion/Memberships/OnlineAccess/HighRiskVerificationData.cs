using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    public enum Action
    {
        Undefined,
        RedirectToProfile,
        RedirectToAch,
        PerformCheckWithdrawal,
        PerformCrossAccountTransfer,
        BillPayBulkPayment,
        BillPaySinglePayment,
        BillPayRecurringPayment
    }

    [DataContract]
    public class HighRiskVerificationData
    {
        [DataMember]
        public string AfterAction { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string ToAccountDetails { get; set; }
        [DataMember]
        public string FromAccountDetails { get; set; }
        [DataMember]
        public string ToAccount { get; set; }
        [DataMember]
        public string FromAccount { get; set; }
        [DataMember]
        public string ToSuffix { get; set; }
        [DataMember]
        public string FromSuffix { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsFromShare { get; set; }
        [DataMember]
        public bool IsToShare { get; set; }
        [DataMember]
        public string BillPayPaymentsJson { get; set; }
        [DataMember]
        public string BillPayRecurringPaymentJson { get; set; }
        [DataMember]
        public string LogData { get; set; }
    }
}
