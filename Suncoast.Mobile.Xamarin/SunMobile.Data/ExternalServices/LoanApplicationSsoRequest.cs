using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;

namespace SunBlock.DataTransferObjects.ExternalServices
{
	[DataContract]
	public class LoanApplicationSsoRequest
	{

		#region Declarations...

		#endregion

		#region Properties...

		/// <summary>
		/// Member ID of the user requesting SSO credentials
		/// </summary>
		[DataMember]
		public int MemberId { get; set; }
		[DataMember]
		public PayloadMessage Payload { get; set; }

		/// <summary>
		/// Pass in MemberInformation to avoid additional calls.
		/// If null, Member Information will be populated from data sources.
		/// </summary>
		[DataMember]
		public MemberInformation MemberInfo { get; set; }

		#endregion

		#region Methods...

		#endregion
	}
}