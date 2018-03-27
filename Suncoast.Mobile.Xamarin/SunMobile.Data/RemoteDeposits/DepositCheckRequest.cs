using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
    [DataContract]
    public class DepositCheckRequest
    {
        [DataMember]
        public string UserNameToken { get; set; }
        [DataMember]
        public string PhoneDescription { get; set; }
        [DataMember]
        public string DepositAccountNumber { get; set; }
        [DataMember]
        public string DepositRoutingNumber { get; set; }
        [DataMember]
        public long AmountInCents { get; set; }
        [DataMember]
        public string FrontImageBase64 { get; set; }
        [DataMember]
        public string BackImageBase64 { get; set; }
        [DataMember]
        public bool ReturnCheckData { get; set; }
        [DataMember]
        public bool ReturnFrontImage { get; set; }
        [DataMember]
        public bool ReturnBackImage { get; set; }
    }
}