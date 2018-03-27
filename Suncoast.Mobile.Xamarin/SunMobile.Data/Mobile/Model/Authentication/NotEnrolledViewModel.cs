using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
    [DataContract]
    public class NotEnrolledViewModel : ViewContext {
        private NotEnrolledResponse _notEnrolledResponse;

        [DataMember]
        public NotEnrolledResponse NotEnrolledResponse
        {
            get
            {
                if (_notEnrolledResponse == null)
                    _notEnrolledResponse = new NotEnrolledResponse();

                return _notEnrolledResponse;
            }

            set
            {
                _notEnrolledResponse = value;
            }
        }
    }
}