using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Session
{
    [DataContract]
    public class GetStartupSettingsResponse
    {
        //[DataMember]
        //public Dictionary<string, string> StartupSettings { get; set; }
        [DataMember]
        public List<string> Keys { get; set; }
        [DataMember]
        public List<string> Values { get; set; } 
    }
}