using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;

namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	[DataContract]
	[KnownType(typeof(AccountSpecificAlertSettings))]
	[KnownType(typeof(AccountSpecificAlertModel))]
	public class AccountSpecificAlertSettingsSet : NotificationSetting, IAccountSpecificAlertSettingsSet
	{
		[DataMember]
		public List<IAccountSpecificAlertSettings> AccountSpecificAlertSettings { get; set; }
	}
}
