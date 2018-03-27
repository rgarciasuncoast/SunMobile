using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.RocketAccounts;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text
{
	[DataContract]
	public class AccountListTextViewModel : ViewContext
	{
		[DataMember]
		public Collection<HeaderSectionTextView<Account>> AccountSections { get; set; }

		[DataMember]
		public RocketEligibility RocketEligibility { get; set; }
	}
}