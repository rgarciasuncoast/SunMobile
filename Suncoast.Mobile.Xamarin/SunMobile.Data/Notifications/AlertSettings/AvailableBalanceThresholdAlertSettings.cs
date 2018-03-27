using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Notifications.AlertSettings
{
	[DataContract]
	public class AvailableBalanceThresholdAlertSettings : NotificationSetting, IAvailableBalanceThresholdAlertSettings
	{
		[DataMember]
		public decimal ThreshHoldAmount { get; set; }
	}
}
