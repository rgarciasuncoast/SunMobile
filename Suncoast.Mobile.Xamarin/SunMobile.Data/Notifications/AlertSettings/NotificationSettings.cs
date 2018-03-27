using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;

namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	[DataContract]
	[KnownType(typeof(AccountSpecificAlertSettingsSet))]
	[KnownType(typeof(AccountSpecificAlertModelSet))]
	[KnownType(typeof(AlertSettingModel))]
	public class NotificationSettings : NotificationSetting, INotificationSettings
	{
		[DataMember]
		public IAccountSpecificAlertSettingsSet AccountSpecificAlertSettingsSet { get; set; }
		[DataMember]
		public INotificationSetting SecurityAlertsSetting { get; set; }
		[DataMember]
		public INotificationSetting EDocumentAlertsSetting { get; set; }
	}
}