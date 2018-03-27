using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class AccountTransactionModel<TListItem> : ListItemModel<TListItem> where TListItem : ListItem
    {
        [DataMember]
        public Transaction TransactionInformation { get; set; }

    }
}