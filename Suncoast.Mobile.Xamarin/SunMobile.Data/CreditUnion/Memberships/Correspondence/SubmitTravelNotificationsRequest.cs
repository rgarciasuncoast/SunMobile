using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Correspondence
{
	[DataContract]
	public class SubmitTravelNotificationsRequest
	{
		[DataMember]
		public List<BankCard> Cards { get; set; }

		[DataMember]
		public string StartDate { get; set; }

		[DataMember]
		public string EndDate { get; set; }

		[DataMember]
		public string Locations { get; set; }

		[DataMember]
		public string MemberName { get; set; }

		[DataMember]
		public int MemberId { get; set; }

		[DataMember]
		public string AdditionalDetails { get; set; }
	}
}