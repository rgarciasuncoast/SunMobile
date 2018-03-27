using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.QueuedProcesses
{
    [DataContract]
    public class TravelNotificationRequestFailure
    {
        [DataMember]
        public TravelNotificationRequest Request { get; set; }
        [DataMember]
        public string MemoRequestError { get; set; }
        [DataMember]
        public string ElectronicFormRequestError { get; set; }
    }
}
