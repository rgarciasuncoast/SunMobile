using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public class AuthenticationToken
    {
        [DataMember]
        public Int32 AppId { get; set; }
        [DataMember]
        public Int32 MemberId { get; set; }
        [DataMember]
        public string TaxIdLastFourDigits { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public DateTime TimeCreated { get; set; }
        [DataMember]
        public DateTime SunnetSsoExpiration { get; set; }
        [DataMember]
        public bool IsSunnetAuthorized { get; set; }
        [DataMember]
        public Int32 DocuSignUserId { get; set; }
    }
}