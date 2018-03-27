using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Fis
{
    [DataContract]
    public class Activity
    {
        [DataMember]
        public string RequestCode { get; set; }
        [DataMember]
        public string OperatorId { get; set; }
        [DataMember]
        public string TerminalId { get; set; }
        [DataMember]
        public string TimeSent { get; set; }
        [DataMember]
        public Int32 BlockLength { get; set; }
        [DataMember]
        public Int32 ScreenNumber { get; set; }
        [DataMember]
        public Int32 MessageSequenceNumber { get; set; }
        [DataMember]
        public string CorporationNumber { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string Asf { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string Filler { get; set; }
        [DataMember]
        public string BillingCycleNumber { get; set; }
        [DataMember]
        public string LastStatementKey { get; set; }
        [DataMember]
        public string StatementKey { get; set; }
        [DataMember]
        public string StolenAccountKey { get; set; }
        [DataMember]
        public DateTime PaymentDueDate { get; set; }
        [DataMember]
        public string BlockReclass { get; set; }
        [DataMember]
        public string StatementGroupCode { get; set; }
        [DataMember]
        public string TotalCtdFinanceCharge { get; set; }
        [DataMember]
        public string BalanceForward { get; set; }
        [DataMember]
        public string NbrPlansBalanceSegs { get; set; }
        [DataMember]
        public string CtdAmountLateCharge { get; set; }
        [DataMember]
        public string TotalNewBalance { get; set; }
        [DataMember]
        public string Purchases { get; set; }
        [DataMember]
        public int BillDay { get; set; }
        [DataMember]
        public string CtdPayments { get; set; }
        [DataMember]
        public string StationId1 { get; set; }
        [DataMember]
        public string StationId2 { get; set; }
        [DataMember]
        public string StationId3 { get; set; }
        [DataMember]
        public string StationId4 { get; set; }
        [DataMember]
        public string StationId5 { get; set; }
        [DataMember]
        public string StationId6 { get; set; }
        [DataMember]
        public string CtdOtherCredits { get; set; }
        [DataMember]
        public string LastMinimumPayment { get; set; }
        [DataMember]
        public string MinimumDue { get; set; }
        [DataMember]
        public string CtdOtherDebits { get; set; }
        [DataMember]
        public string CreditLimit { get; set; }
        [DataMember]
        public string PayAheadAmount { get; set; }
        [DataMember]
        public int StatementLineCount { get; set; }
        [DataMember]
        public Collection<Lines> StatementLines { get; set; }
        [DataMember]
        public static string LastStatementMarker { get; set; }
    }
}
