using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
	[DataContract]
	public class EnrollFingerprintAuthorizationResponse : StatusResponse
	{
		[DataMember]
		public string FingerprintAuthorizationCode { get; set; }
	}
}