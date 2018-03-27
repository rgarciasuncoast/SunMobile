using System.Runtime.Serialization;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
	public enum FlagTypes
	{
		NotDefined,
		SunnetEnrolled,
		BillpayEnrolled,
		AlertsEnrolled,
		ENoticesEnrolled,
		EStatementsEnrolled,
		CCEStatementsEnrolled,
		MortgageEStatementsEnrolled,
		AchEnrolled,
		PfmEnrolled,
		QuickenEnrolled,
		RdcEligibilityFlag,
		RdcRestrictFlag,
		SpanishEStatementsEnrolled,
		SunTextEnrolled,
		OnlineDisclosureAccepted,
		EStatementOptinViewed,
		InstantCheckingMemberDeclined
	}

	[DataContract]
	public class FlagRequest
	{
		[DataMember]
		public string MemberId { get; set; }

		[DataMember]
		public bool Value { get; set; }

		[DataMember]
		public FlagTypes FlagType { get; set; }
	}
}
