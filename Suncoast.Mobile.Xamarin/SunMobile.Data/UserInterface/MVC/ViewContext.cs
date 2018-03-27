using System.Runtime.Serialization;
namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    [DataContract]
    public class ViewContext
    {
        [DataMember]
        public string ClientViewState { get; set; }
        [DataMember]
        public string InitialClientMessagePrompt { get; set; }
        [DataMember]
        public string ClientMessage { get; set; }
        
    }
}
