using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Notifications.AlertSettings;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts
{
	[DataContract]
	public class AvailableBalanceThresholdAlertModel : AvailableBalanceThresholdAlertSettings, IDisplayText, IAdditionalSettings
	{
		[DataMember]
		public string DisplayText { get; set; }
		[DataMember]
		public bool AdditionalSettings { get; set; }
	}
}