using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Notifications;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts
{
	[DataContract]
	public class AlertSettingModel : NotificationSetting, IDisplayText
	{
		[DataMember]
		public string DisplayText { get; set; }
	}
}
