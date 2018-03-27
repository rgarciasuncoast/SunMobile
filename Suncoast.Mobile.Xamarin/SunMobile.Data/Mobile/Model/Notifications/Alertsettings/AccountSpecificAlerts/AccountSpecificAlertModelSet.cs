using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Notifications.AlertSettings;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts
{
	[DataContract]
	public class AccountSpecificAlertModelSet : AccountSpecificAlertSettingsSet, IDisplayText
	{
		[DataMember]
		public string DisplayText { get; set; }

	}
}
