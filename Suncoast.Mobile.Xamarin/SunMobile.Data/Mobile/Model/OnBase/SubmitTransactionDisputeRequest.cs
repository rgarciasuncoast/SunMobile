using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.OnBase
{
	[DataContract]
	public class SubmitTransactionDisputeRequest
	{
		[DataMember]
		public int Reason { get; set; }
		[DataMember]
		public string DisputeType { get; set; }
		[DataMember]
		public List<string> Answers { get; set; }
		[DataMember]
		public List<string> DocumentIds { get; set; }
		[DataMember]
		public string CardNumber { get; set; }
		[DataMember]
		public string Suffix { get; set; }
		[DataMember]
		public string CardholderName { get; set; }
		[DataMember]
		public string AddressLine1 { get; set; }
		[DataMember]
		public string AddressLine2 { get; set; }
		[DataMember]
		public string City { get; set; }
		[DataMember]
		public string State { get; set; }
		[DataMember]
		public string UnmaskedZipCode { get; set; }
		[DataMember]
		public string DaytimePhone { get; set; }
		[DataMember]
		public List<AdditionalTransactionInfo> AdditionalTransactions { get; set; }
	}
}