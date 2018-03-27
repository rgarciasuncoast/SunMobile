using System.Runtime.Serialization;
using SunBlock.DataTransferObjects;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
    [DataContract]
    public class RocketCheckingResponse : StatusResponse
    {
        [DataMember]
        public bool Has88Warning { get; set; } // When true, member changed addresss in the last 2 Days, warn member of 2 day delay in processing
    }
}