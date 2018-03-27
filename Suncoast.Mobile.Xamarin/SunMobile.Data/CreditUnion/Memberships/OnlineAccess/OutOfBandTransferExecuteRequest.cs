using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class OutOfBandTransferExecuteRequest : TransferExecuteRequest
	{
		[DataMember]
		public string OutOfBandTransactionId { get; set; }
		[DataMember]
		public string VerificationCode { get; set; }
	}
}