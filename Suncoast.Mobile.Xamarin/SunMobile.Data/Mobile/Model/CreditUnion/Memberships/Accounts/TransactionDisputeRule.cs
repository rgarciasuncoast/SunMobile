using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransactionDisputeRule
	{
		[DataMember]
		public string TransactionCode { get; set; }
		[DataMember]
		public List<string> DisputeReasons { get; set; }
	}
}