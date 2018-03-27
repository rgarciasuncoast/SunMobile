using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
    [DataContract]
    public class AccountCodeMapping
    {
        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public string ShareOrLoan { get; set; }

        [DataMember]
        public int HostCode { get; set; }

        [DataMember]
        public bool PermitTransferSource { get; set; }

        [DataMember]
        public bool PermitTransferTarget { get; set; }

        [DataMember]
        public bool PermitCheckWithdrawal { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string AccountType { get; set; }
    }
}
