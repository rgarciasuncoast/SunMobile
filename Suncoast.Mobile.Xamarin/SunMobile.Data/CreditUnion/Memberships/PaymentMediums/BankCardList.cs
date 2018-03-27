using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums
{
    [DataContract]
    public class BankCardList: StatusResponse
    {
        [DataMember]
        public List<BankCard> BankCards { get; set; } 
    }
}
