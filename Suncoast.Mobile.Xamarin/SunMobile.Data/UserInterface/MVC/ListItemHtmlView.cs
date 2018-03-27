using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    [DataContract]
    public class ListItemHtmlView: ListItem
    {
        [DataMember]
        public string ViewHtml { get; set; }
        [DataMember]
        public string SubViewHtml { get; set; }
    }
}
