using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;

namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	[DataContract]
	[KnownType(typeof(AvailableBalanceThresholdAlertSettings))]
	[KnownType(typeof(AvailableBalanceThresholdAlertModel))]
	[KnownType(typeof(AlertSettingModel))]
	[KnownType(typeof(NotificationSetting))]
	public class AccountSpecificAlertSettings : NotificationSetting, IAccountSpecificAlertSettings
	{
		[DataMember]
		public string AccountId { get; set; }

		[DataMember]
		public string AccountSettingType { get; set; }

		[DataMember]
		public IAvailableBalanceThresholdAlertSettings AvailableBalaceThresholdAlertSettings { get; set; }

		[DataMember]
		public INotificationSetting PaymentReminderAlertSettings { get; set; }

		[DataMember]
		public INotificationSetting NsfAlertSettings { get; set; }

		[DataMember]
		public INotificationSetting DirectDepositAlertSettings { get; set; }
	}
}