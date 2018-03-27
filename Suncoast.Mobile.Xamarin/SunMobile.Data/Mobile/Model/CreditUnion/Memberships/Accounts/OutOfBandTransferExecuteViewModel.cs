using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class OutOfBandTransferExecuteViewModel : ViewContext
	{
		[DataMember]
		public string OutOfBandTransactionId { get; set; }
		[DataMember]
		public List<NotificationOption> NotificationOptions { get; set; }
		[DataMember]
		public bool HasCreditCard { get; set; }
	}
}