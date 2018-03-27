using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay.V2
{
	[DataContract]
	public class Payee
	{
		#region Declarations...

		#endregion

		#region Properties...

		[DataMember]
		public long MemberPayeeId { get; set; }

		[DataMember]
		public string PayeeName { get; set; }

		[DataMember]
		public string PayeeAlias { get; set; }

		[DataMember]
		public string PayeeAccountNumber { get; set; }

		[DataMember]
		public string NameOnAccount { get; set; }

		[DataMember]
		public string Address1 { get; set; }

		[DataMember]
		public string Address2 { get; set; }

		[DataMember]
		public string City { get; set; }

		[DataMember]
		public string State { get; set; }

		[DataMember]
		public string PostalCode { get; set; }

		[DataMember]
		public string Phone { get; set; }

		[DataMember]
		public string DeliveryMethod { get; set; }

		[DataMember]
		public string DeliveryDays { get; set; }

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public int MemberId { get; set; }

		[DataMember]
		public string SourceApplication { get; set; }

		#endregion

		#region Methods...

		#endregion
	}
}