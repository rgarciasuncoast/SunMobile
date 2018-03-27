using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class CreditCardDocumentRequest : DocumentRequest
    {
        [DataMember]
        public string CreditCardNumber { get; set; }
    }
}
