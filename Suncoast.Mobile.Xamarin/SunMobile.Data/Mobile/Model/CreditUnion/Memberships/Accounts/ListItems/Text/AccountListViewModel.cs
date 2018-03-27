using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text
{
	[DataContract]
	public class AccountListViewModel : ViewContext
	{
		[DataMember]
		public AccountList Accounts { get; set; }
	}
}