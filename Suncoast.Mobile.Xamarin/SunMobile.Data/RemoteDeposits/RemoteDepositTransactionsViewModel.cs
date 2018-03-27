using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class RemoteDepositTransactionsViewModel : ViewContext
    {
        [DataMember]
        public Collection<RemoteDepositTransaction> RemoteDepositTransactions { get; set; }
    }
}