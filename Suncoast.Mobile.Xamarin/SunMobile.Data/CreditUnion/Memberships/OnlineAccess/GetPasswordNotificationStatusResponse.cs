using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class GetPasswordNotificationStatusResponse
	{
		[DataMember]
		public bool ShouldShowUpdateNotification { get; set; }
		[DataMember]
		public DateTime LastPasswordChangeDateUtc { get; set; }
		[DataMember]
		public DateTime LastPasswordPostponementDateUtc { get; set; }
		[DataMember]
		public DateTime NextReminderDate { get; set; }
	}
}