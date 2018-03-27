using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.UserInterface.MVC;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
    [DataContract]
    public class UpdateImagePhraseViewModel : ViewContext {
        [DataMember]
        public UpdateImagePhraseResponse UpdateImagePhraseResponse { get; set; }
    }
}