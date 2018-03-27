using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransactionDisputeAnswer
	{
		[DataMember]
		public string Answer { get; set; }
		[DataMember]
		public string AnswerType { get; set; }   // string, bool, datetime, decimal, int, list
		[DataMember]
		public int AnswerSelection { get; set; }
	}
}