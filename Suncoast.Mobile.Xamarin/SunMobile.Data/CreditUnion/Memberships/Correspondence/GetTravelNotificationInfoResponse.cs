using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Correspondence
{
	[DataContract]
	public class GetTravelNotificationInfoResponse : StatusResponse
	{
		[DataMember]
		public string AdditionalInfoUrl { get; set; }
	}
}