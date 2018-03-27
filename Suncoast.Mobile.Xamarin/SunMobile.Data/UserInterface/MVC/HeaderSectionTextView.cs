using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    [DataContract]
    public class HeaderSectionTextView<T>
    {
        [DataMember]
        public string HeaderText { get; set; }
        [DataMember]
        public Collection<ItemSectionTextView<T>> Items { get; set; }
    }
}
