using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums
{
    [DataContract]
    public class CustomCardRequest
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public string CustomCardType { get; set; }
        [DataMember]
        public string ReplacementNames { get; set; }
        [DataMember]
        public string CardHolderName { get; set; }
        [DataMember]
        public bool IsDebitCard { get; set; }
    }
}