using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts
{
	[DataContract]
	public class AccountSpecificAlertSettingsUpdateRequest
	{
		[DataMember]
		public string Suffix { get; set; }

		[DataMember]
		public string AccountSettingType { get; set; }

		[DataMember]
		public bool Value { get; set; }
	}
}