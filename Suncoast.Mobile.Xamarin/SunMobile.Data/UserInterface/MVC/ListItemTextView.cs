using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    public class ListItemTextView
    {
        [DataMember]
        public Collection<string> DisplayFieldNames { get; set; }
        [DataMember]
        public Collection<string> DisplayFieldValues { get; set; } 
    }
}
