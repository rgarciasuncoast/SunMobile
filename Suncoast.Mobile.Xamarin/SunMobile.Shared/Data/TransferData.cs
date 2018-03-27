using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;

namespace SunMobile.Shared.Data
{
	public class TransferData
	{
		public TransferTarget SourceTarget { get; set; }
		public TransferTarget DestinationTarget { get; set; }
		public bool IsJoint { get; set; }
		public bool IsAnyMember { get; set; }
		public string LastName { get; set; }
		public bool OtherFlag { get; set; }
		public string SourceHeader { get; set; }
		public string SourceDetail { get; set; }
		public string DesinationHeader { get; set; }
		public string DestinationDetail { get; set; }
		public string ListViewItem { get; set; }
	}
}