using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    [DataContract]
    public class BiometricInformationRequest
    {
        [DataMember]
        public string DeviceType { get; set; }
        [DataMember]
        public string BiometricType { get; set; } // BiometricTypes enum (Fingerprint, Face)
    }
}