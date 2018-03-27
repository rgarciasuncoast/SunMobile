using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    [DataContract]
    public class ListItem
    {

        [DataMember]
        public string SucceedingClientViewState { get; set; }

        [DataMember]
        public string SucceedingClientViewArguments { get; set; }
    }
}