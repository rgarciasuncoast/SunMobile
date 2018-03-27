using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile
{
    [DataContract]
    public class GetInAuthLogConfigResponse : StatusResponse
    {   
        [DataMember]
        public string DeviceResponse { get; set; }
        [DataMember]
        public string DateLastChanged { get; set; } //yyyyMMdd
    }
}