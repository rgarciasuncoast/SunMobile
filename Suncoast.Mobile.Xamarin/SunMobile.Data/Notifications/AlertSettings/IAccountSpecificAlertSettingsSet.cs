using System.Collections.Generic;

namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	public interface IAccountSpecificAlertSettingsSet : INotificationSetting
	{
		List<IAccountSpecificAlertSettings> AccountSpecificAlertSettings { get; set; }
	}
}
