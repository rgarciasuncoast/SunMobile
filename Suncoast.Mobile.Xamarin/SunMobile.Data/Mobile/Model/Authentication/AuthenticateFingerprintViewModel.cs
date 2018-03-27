using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications;
using SunBlock.DataTransferObjects.UserInterface.MVC;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;

namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
    [DataContract]
    public class AuthenticateFingerprintViewModel : ViewContext
    {
        [DataMember]
        public AuthenticateFingerprintResponse AuthenticateFingerprintResponse { get; set; }
        [DataMember]
        public MobileLoginResponse MobileLoginResponse { get; set; }
        [DataMember]
        public NotificationRegistrationData NotificationData { get; set; }
        [DataMember]
        public bool CanUseAtmLastEight { get; set; }
    }
}