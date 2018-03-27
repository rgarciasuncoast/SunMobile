using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class ExecuteResponse : SunBlock.DataTransferObjects.StatusResponse
    {
        [DataMember]
        public Collection<ImageDocument> Documents { get; set; }
    }
}