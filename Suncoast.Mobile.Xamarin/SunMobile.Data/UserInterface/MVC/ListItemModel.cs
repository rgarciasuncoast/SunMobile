using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SunBlock.DataTransferObjects.UserInterface.MVC
{
    [DataContract]
    public class ListItemModel<T> where T:ListItem
    {
        [DataMember]
        public T ListItem { get; set; }
    }
}
