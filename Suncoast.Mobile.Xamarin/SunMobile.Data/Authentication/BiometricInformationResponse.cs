using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    [DataContract]
    public class BiometricInformationResponse : StatusResponse
    {
        [DataMember]
        public string AgreementText { get; set; }
    }
}