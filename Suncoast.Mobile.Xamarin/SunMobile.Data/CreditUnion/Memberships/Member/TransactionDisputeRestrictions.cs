using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Member
{
    [DataContract]
    public class TransactionDisputeRestrictions
    {
        [DataMember]
        public bool IsFrequentDisputer { get; set; }
        [DataMember]
        public string FrequentDisputeInstructions { get; set; }
    }
}