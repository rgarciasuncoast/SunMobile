using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Html
{
    [DataContract]
    public class AccountListHtmlViewModel : ViewContext
    {
        [DataMember]
        public Collection<AccountHtmlListItemModel> Accounts { get; set; }
        
    }
}
