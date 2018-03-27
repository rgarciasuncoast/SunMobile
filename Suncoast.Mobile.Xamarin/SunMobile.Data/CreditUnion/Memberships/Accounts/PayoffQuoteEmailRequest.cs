using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Products;

namespace SunMobile.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class PayoffQuoteEmailRequest
    {
        [DataMember]
        public PayoffQuoteResponse PayoffQuote { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}