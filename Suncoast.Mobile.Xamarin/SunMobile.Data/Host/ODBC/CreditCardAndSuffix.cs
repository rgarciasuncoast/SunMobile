using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.ODBC
{
    [DataContract]
    public class CreditCardAndSuffix
    {
        [DataMember]
        public string Suffix { get; set; }

        [DataMember]
        public string CreditCardNumber { get; set; }
    }
}
