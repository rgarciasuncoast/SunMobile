using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.UserInterface.MVC;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
    [DataContract]
    public class AuthenticateHostViewModel : ViewContext
    {
        [DataMember]
        public AuthenticateHostResponse AuthenticateHostResponse { get; set; }
        [DataMember]
        public MobileLoginResponse MobileLoginResponse { get; set; }
        [DataMember]
        public bool CanUseAtmLastEight { get; set; }
    }
}