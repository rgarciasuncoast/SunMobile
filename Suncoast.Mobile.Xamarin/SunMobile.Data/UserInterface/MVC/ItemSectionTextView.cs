using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    public class ItemSectionTextView<T> : ListItem
    {
        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public ListItemTextView ListItems { get; set; }
    }
}
