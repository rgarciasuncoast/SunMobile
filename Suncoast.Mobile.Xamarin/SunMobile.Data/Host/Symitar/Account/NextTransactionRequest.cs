using System.Runtime.Serialization;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
	[DataContract]
	public class NextTransactionRequest
	{
		[DataMember]
		public int MemberContext { get; set; }
		[DataMember]
		public string LastTransactionDate { get; set; } //yyyyMMdd
		[DataMember]
		public string LastTransactionIdentifier { get; set; }
		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public string Suffix { get; set; }
		[DataMember]
		public string TransactionAccountType { get; set; } //TransactionAccountTypes enum
	}
}