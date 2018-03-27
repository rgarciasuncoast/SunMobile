using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class RemoteDepositTransactionResponse
    {
        [DataMember]
        public Collection<RemoteDepositTransaction> RemoteDepositTransactions { get; set; }
        [DataMember]
        public bool IsSuccessful { get; set; }
    }
}
