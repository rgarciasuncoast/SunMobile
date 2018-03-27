using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class SmsSettingsRedirectResponse : StatusResponse
    {
        [DataMember]
        public string RedirectUrl { get; set; }

    }
}