using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.Notifications
{
	[DataContract]
	public class NotificationSetting : INotificationSetting
	{
		[Queryable]
		[DataMember]
		public bool Enabled { get; set; }
	}
}