using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.UserInterface.MVC;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
    [DataContract]
    public class UnlockViewModel : ViewContext {
        [DataMember]
        public UnlockResponse UnlockResponse { get; set; }
    }
}