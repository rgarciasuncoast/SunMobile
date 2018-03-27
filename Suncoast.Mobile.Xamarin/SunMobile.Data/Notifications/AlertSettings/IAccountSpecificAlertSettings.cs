namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	public interface IAccountSpecificAlertSettings : INotificationSetting
	{
		string AccountId { get; set; }

		string AccountSettingType { get; set; }

		IAvailableBalanceThresholdAlertSettings AvailableBalaceThresholdAlertSettings { get; set; }

		INotificationSetting PaymentReminderAlertSettings { get; set; }

		INotificationSetting NsfAlertSettings { get; set; }

		INotificationSetting DirectDepositAlertSettings { get; set; }
	}
}