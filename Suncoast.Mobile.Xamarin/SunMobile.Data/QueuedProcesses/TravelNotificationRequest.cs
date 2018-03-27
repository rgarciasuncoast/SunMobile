using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Host.Pathways;
using SunBlock.DataTransferObjects.OnBase;

namespace SunBlock.DataTransferObjects.QueuedProcesses
{
    [DataContract]
    public class TravelNotificationRequest
    {
        [DataMember]
        public InsertAtmDebitMemoRequest MemoRequest { get; set; }
        [DataMember]
        public SubmitElectronicFormRequest ElectronicFormRequest { get; set; }
    }
}
