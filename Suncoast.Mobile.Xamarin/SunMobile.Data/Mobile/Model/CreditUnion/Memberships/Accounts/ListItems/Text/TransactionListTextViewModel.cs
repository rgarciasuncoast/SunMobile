using SunBlock.DataTransferObjects.UserInterface.MVC;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text
{
    [DataContract]
    public class TransactionListTextViewModel : ViewContext
    {
        [DataMember]
        public Collection<HeaderSectionTextView<Transaction>> TransactionSections { get; set; }
    }
}
