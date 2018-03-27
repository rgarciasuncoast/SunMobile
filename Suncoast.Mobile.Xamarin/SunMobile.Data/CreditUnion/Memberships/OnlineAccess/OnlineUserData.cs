using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class OnlineUserData
    {
        [Queryable]
        [SensitiveData]
        [DataMember]
        public string MemberId { get; set; }

        [Queryable]
        [DataMember]
        public bool HighRiskVerify { get; set; }

        [Queryable]
        [DataMember]
        public int HighRiskTokenLockoutCounter { get; set; }

        [Queryable]
        [DataMember]
        public int HighRiskLast8LockoutCounter { get; set; }

        [Queryable]
        [DataMember]
        public DateTime HighRiskBeginTimeUtc { get; set; }

        [Queryable]
        [DataMember]
        public DateTime HighRiskExpireTimeUtc { get; set; }

        [Queryable]
        [DataMember]
        public DateTime HighRiskTokenLockoutExpireTimeUtc { get; set; }

        [Queryable]
        [DataMember]
        public DateTime HighRiskLast8LockoutExpireTimeUtc { get; set; }

        [Queryable]
        [DataMember]
        public bool HighRiskLast8Lockout { get; set; }

        /// <summary>
        /// Value will be null if no Alert Settings exist for the member.
        /// </summary>
        [DataMember]
        public AlertSettings AlertSettings { get; set; }

        [DataMember]
        public bool DisableAutoPinChange { get; set; }

        [DataMember]
        public List<PinHistoryItem> PinHistory { get; set; }

        [DataMember]
        public EnrollmentData AchTransfers { get; set; }

        [DataMember]
        public EnrollmentData FinancialManagement { get; set; }

        [DataMember]
        public string AutoLaunchPage { get; set; }

        [DataMember]
        public DateTime CurrentOnlineAccessTime { get; set; }

        [DataMember]
        public DateTime LastOnlineAccessTime { get; set; }

        [DataMember]
        public int LoginCount { get; set; }

        [DataMember]
        public bool OnlineBankingEnrollmentDisplayedToUser { get; set; }

        [DataMember]
        public string LastUserAction { get; set; }

        [DataMember]
        public EnrollmentData SunMoneyEnrollment { get; set; }

        [DataMember]
        public EnrollmentData RemoteDepositEnrollment { get; set; }

        [DataMember]
        public EnrollmentData RemoteDepositSmsAlertEnrollment { get; set; }

        [DataMember]
        public DateTime PinResetProcessTokenExpiration { get; set; }

        [DataMember]
        public bool PinResetProcessStarted { get; set; }

        [DataMember]
        public string PinResetProcessSecurityToken { get; set; }

        [DataMember]
        public string PinResetProcessAuthorizationToken { get; set; }

        [DataMember]
        public int PinResetProcessNumberOfTries { get; set; }

        [DataMember]
        public int PinResetProcessTotalNumberOfResets { get; set; }

        [DataMember]
        public DateTime ProfileLastViewDate { get; set; }

        [DataMember]
        public string BillPayDefaultSourceAccount { get; set; }

        [DataMember]
        public List<UserPreference> UserPreferences { get; set; }
    }
}
