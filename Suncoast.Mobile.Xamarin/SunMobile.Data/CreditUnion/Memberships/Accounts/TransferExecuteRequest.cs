using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransferExecuteRequest
	{
		[DataMember]
		public TransferTarget Source { get; set; }
		[DataMember]
		public TransferTarget Destination { get; set; }
		[DataMember]
		public decimal Amount { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string LastEightOfAtmDebitCard { get; set; } //TODO: can we get rid of this
		[DataMember]
		public bool IsJointTransfer { get; set; } //TODO: this is view logic. We need to show the member a list of available joint accounts and get rid of this field
		[DataMember]
		public int RequestingMemberId { get; set; }
		[DataMember]
		public bool UserOverride { get; set; }
		[DataMember]
		public PayloadMessage Payload { get; set; }
	}
}
