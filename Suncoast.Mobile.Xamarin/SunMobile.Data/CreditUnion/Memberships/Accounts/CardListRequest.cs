using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class CardListRequest
	{
		[DataMember]
		public bool ExcludeAtmCards { get; set; }
		[DataMember]
		public bool IncludeClosedCards { get; set; }
	}
}