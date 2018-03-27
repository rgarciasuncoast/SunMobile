using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class AccountModel<TListItem> : ListItemModel<TListItem> where TListItem :ListItem
    {
        [DataMember]
        public Account AccountInformation { get; set; }
    }
}
