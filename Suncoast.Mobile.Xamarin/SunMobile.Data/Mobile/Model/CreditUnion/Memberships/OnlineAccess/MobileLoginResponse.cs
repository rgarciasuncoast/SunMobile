using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class MobileLoginResponse : StatusResponse
    {
        [DataMember]
        public bool EStatementOptInViewed { get; set; }
        [DataMember]
        public bool EStatementsEnrolled { get; set; }
        [DataMember]
        public string EStatementAgreementText { get; set; }
        [DataMember]
        public bool IsOnlineDisclosureAccepted { get; set; }
        [DataMember]
        public string OnlineBankingAgreementText { get; set; }
        [DataMember]
        public bool ShouldShowUpdateNotification { get; set; }
        [DataMember]
        public DateTime LastPasswordChangeDateUtc { get; set; }
        [DataMember]
        public DateTime LastPasswordPostponementDateUtc { get; set; }
        [DataMember]
        public DateTime NextReminderDate { get; set; }
        [DataMember]
        public bool HostIsUnavailable { get; set; }
    }
}