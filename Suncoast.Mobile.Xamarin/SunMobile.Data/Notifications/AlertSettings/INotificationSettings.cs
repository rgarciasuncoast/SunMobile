namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	public interface INotificationSettings : INotificationSetting
	{
		IAccountSpecificAlertSettingsSet AccountSpecificAlertSettingsSet { get; set; }
	}
}
