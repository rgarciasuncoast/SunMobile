using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.OnBase
{
    [DataContract]
    public class CheckImagesViewModel: ViewContext
    {
        [DataMember]
        public int DocumentId { get; set; }
        [DataMember]
        public string DocumentName { get; set; }
        [DataMember]
        public string FrontImage { get; set; }
        [DataMember]
        public string BackImage { get; set; }
    }
}
