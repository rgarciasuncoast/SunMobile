namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	public interface IAvailableBalanceThresholdAlertSettings : INotificationSetting
	{
		decimal ThreshHoldAmount { get; set; }
	}
}
