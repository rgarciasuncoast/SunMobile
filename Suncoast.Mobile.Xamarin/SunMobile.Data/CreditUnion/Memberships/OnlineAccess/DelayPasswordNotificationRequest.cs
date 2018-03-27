using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class DelayPasswordNotificationRequest
	{
		[DataMember]
		public int MemberId { get; set; }
	}
}