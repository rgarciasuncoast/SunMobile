using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class OutOfBandTransferExecuteResponse : TransactionResponse
	{
		[DataMember]
		public string OutOfBandTransactionId { get; set; }
		[DataMember]
		public string AlertType { get; set; }
		[DataMember]
		public string AlertDestination { get; set; }
	}
}