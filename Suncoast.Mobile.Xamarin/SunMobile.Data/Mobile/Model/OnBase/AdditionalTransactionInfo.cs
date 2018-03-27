using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.OnBase
{
	[DataContract]
	public class AdditionalTransactionInfo
	{
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public DateTime TransactionDate { get; set; }
		[DataMember]
		public string TransactionAmount { get; set; }
		[DataMember]
		public string TransactionType { get; set; }
		[DataMember]
		public int TransactionId { get; set; }
	}
}