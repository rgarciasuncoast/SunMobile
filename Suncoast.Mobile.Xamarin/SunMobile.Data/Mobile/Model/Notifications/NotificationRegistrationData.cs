using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications
{
	[DataContract]
	public class NotificationRegistrationData : PSNRegistrationData
	{
		[DataMember]
		public string RegistrationId { get; set; }
	}
}