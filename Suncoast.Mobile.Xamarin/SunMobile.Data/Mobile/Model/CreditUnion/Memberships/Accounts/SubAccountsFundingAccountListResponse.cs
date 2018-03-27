using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class SubAccountsFundingAccountListResponse
	{
		[DataMember]
		public AccountListTextViewModel AccountListViewModel { get; set; }
		[DataMember]
		public decimal RequiredFundingAmount { get; set; }
	}
}