using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
    [DataContract]
    public class PayoffQuoteRequest
    {
        [DataMember]
        public string DateOfPayoff { get; set; }
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string LoanSuffix { get; set; }
    }
}