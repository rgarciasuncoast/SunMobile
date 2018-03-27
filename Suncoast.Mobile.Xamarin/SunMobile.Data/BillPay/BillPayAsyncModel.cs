using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class BillPayAsyncModel: StatusResponse
    {
        #region Declarations...

        #endregion

        #region Properties...
        [DataMember]
        public GetRecurringPaymentFrequenciesResponse RecurringFrequenciesResponse { get; set; }

        [DataMember]
        public StatusResponse<List<ShareAccount>> BillPaySourceAccountsResponse { get; set; }

        [DataMember]
        public StatusResponse<string> DefaultSourceAccountResponse { get; set; }

        [DataMember]
        public GetPaymentListResponse PendingPaymentsResponse { get; set; }

        [DataMember]
        public GetUserPayeeResponse ActiveUserPayeeResponse { get; set; }

        [DataMember]
        public GetUserPayeeResponse InactiveUserPayeeResponse { get; set; }

        [DataMember]
        public GetPaymentListResponse HistoryPaymentsResponse { get; set; }

        [DataMember]
        public GetPaymentRecentActivityResponse RecentPaymentsResponse { get; set; }

        [DataMember]
        public GetRecurringPaymentsResponse RecurringPaymentsResponse { get; set; }

        
        [DataMember]
        public bool RequestInitializationData{ get; set; } 
        [DataMember]
        public bool RequestPendingPayments{ get; set; }
        [DataMember]
        public bool RequestRecurringPayments{ get; set; }
        [DataMember]
        public  bool RequestRecentPayments{ get; set; } 
        [DataMember]
        public bool RequestPaymentHistory{ get; set; } 
        [DataMember]
        public bool RequestPayees{ get; set; }
        [DataMember]
        public int MemberId { get; set; }
        #endregion

        #region Methods...

        #endregion
    }
}
