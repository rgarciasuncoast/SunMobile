using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class GetPasswordNotificationStatusRequest
	{
		[DataMember]
		public int MemberId { get; set; }
	}
}