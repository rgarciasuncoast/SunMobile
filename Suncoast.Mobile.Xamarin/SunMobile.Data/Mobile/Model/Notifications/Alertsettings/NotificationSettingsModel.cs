using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Notifications.AlertSettings;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings
{
	[DataContract]
	public class NotificationSettingsModel : NotificationSettings, IDisplayText
	{
		[DataMember]
		public string DisplayText { get; set; }
		[DataMember]
		public string EnabledDisplayText { get; set; }
	}
}
