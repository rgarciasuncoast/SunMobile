using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts
{
	[DataContract]
	public class AvailableBalanceThresholdSettingsUpdateRequest : AccountSpecificAlertSettingsUpdateRequest
	{
		[DataMember]
		public decimal ThresholdAmount { get; set; }
	}
}