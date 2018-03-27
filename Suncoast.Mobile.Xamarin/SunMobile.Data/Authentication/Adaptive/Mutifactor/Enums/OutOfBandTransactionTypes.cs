using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums
{
	[DataContract]
	public enum OutOfBandTransactionTypes
	{
		[EnumMember]
		Login,
		[EnumMember]
		Profile,
		[EnumMember]
		HighRiskTransfer,
		[EnumMember]
		RemoteDeposit,
		[EnumMember]
		BillPayment,
		[EnumMember]
		RocketAccounts
	}
}