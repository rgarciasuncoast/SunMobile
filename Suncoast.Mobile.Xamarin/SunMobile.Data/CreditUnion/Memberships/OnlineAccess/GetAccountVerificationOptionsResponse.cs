using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class GetAccountVerificationOptionsResponse : StatusResponse
	{
		[DataMember]
		public List<NotificationOption> NotificationOptions { get; set; }
		[DataMember]
		public bool HasCreditCard { get; set; }
	}
}