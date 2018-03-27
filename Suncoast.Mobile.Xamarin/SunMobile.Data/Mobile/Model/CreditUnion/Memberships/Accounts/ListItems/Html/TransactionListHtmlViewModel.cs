using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Html
{
    [DataContract]
    public class TransactionListHtmlViewModel : ViewContext
    {
        [DataMember]
        public Collection<AccountTransactionModel<ListItemHtmlView>> AccountTransactions { get; set; }
    }
}
