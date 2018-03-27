using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion
{
    [DataContract]
    public class TransactionResponse
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
